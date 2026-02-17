using CureLogix.Business.Abstract;
using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CureLogix.WebUI.Controllers
{
    [AllowAnonymous] // Giriş yapmamış kullanıcı erişebilir
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager; // Rol oluşturmak için eklendi
        private readonly IAuditLogService _auditService;
        private readonly IConfiguration _configuration;

        public LoginController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IAuditLogService auditService,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _auditService = auditService;
            _configuration = configuration;
        }

		[HttpGet]
		public async Task<IActionResult> Index(string ReturnUrl, string auto)
		{
			// 1. Kullanıcı zaten içerideyse ana sayfaya at
			if (User.Identity?.IsAuthenticated == true)
			{
				return RedirectToAction("Index", "Home");
			}

			// 🛡️ 2. LİNK İLE OTOMATİK GİRİŞ (PORTAL ENTEGRASYONU)
			// Sadece Frankfurt/Render (Showcase) modundaysak ve ?auto=visitor yazıyorsa çalışır
			bool isShowcase = _configuration.GetValue<bool>("AppSettings:IsShowcaseMode");

			if (isShowcase && auto == "visitor")
			{
				// Şifre sormadan 'User' hesabıyla (CureLogix123!) otomatik giriş
				var result = await _signInManager.PasswordSignInAsync("User", "CureLogix123!", false, false);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}
			}

			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(string username, string password, string ReturnUrl)
		{
			// 🛡️ 1. GİZLİ ANAHTAR ÇÖZÜMLEME
			// Canlıda Render'daki gizli şifreyi, yerelde standart şifreyi (CureLogix123!) kullanır.
			string secureAdminPass = Environment.GetEnvironmentVariable("LIVE_ADMIN_PASSWORD") ?? "CureLogix123!";

			try
			{
				// 🛡️ 2. SELF-HEALING (ADMİN ONARMA)
				if (username == "Admin")
				{
					var existingAdmin = await _userManager.FindByNameAsync("Admin");
					if (existingAdmin == null)
					{
						if (!await _roleManager.RoleExistsAsync("Admin"))
							await _roleManager.CreateAsync(new AppRole { Name = "Admin" });

						var newAdmin = new AppUser
						{
							UserName = "Admin",
							Email = "admin@curelogix.com",
							EmailConfirmed = true,
							NameSurname = "Sistem Yöneticisi"
						};

						await _userManager.CreateAsync(newAdmin, secureAdminPass);
						await _userManager.AddToRoleAsync(newAdmin, "Admin");
					}
				}

				// 🛡️ 3. STANDART GİRİŞ (Identity Doğrulama)
				var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

				if (result.Succeeded)
				{
					// Başarılı Giriş Logu
					try
					{
						_auditService.TAdd(new Entity.Concrete.AuditLog
						{
							UserName = username,
							Activity = "Sisteme giriş yapıldı.",
							Date = DateTime.Now,
							IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"
						});
					}
					catch { }

					if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)) return Redirect(ReturnUrl);
					return RedirectToAction("Index", "Home");
				}

				ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
				return View();
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Sistem Hatası: " + ex.Message;
				return View();
			}
		}

		public async Task<IActionResult> LogOut()
        {
            var username = User.Identity?.Name ?? "Bilinmiyor";

            await _signInManager.SignOutAsync();

            // 🚪 ÇIKIŞ LOGU
            try
            {
                _auditService.TAdd(new AuditLog
                {
                    UserName = username,
                    Activity = "Kullanıcı güvenli çıkış yaptı.",
                    Date = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"
                });
            }
            catch { /* Sessiz kal */ }

            return RedirectToAction("Index", "Login");
        }
    }
}