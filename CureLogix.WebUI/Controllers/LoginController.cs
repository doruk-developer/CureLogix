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
			// 1. Kullanıcı zaten içerideyse direkt ana sayfaya at
			if (User.Identity?.IsAuthenticated == true)
			{
				return RedirectToAction("Index", "Home");
			}

			// ============================================================
			// 🚀 OTOMATİK GİRİŞ KAPISI (PORTAL ENTEGRASYONU)
			// ============================================================
			// Bu "// ===" çizgileri sadece yorum satırıdır, kodun okunabilirliğini artırır.
			// İşleve bir etkisi yoktur, silsen de çalışır ama böyle düzenli durur.

			bool isShowcase = _configuration.GetValue<bool>("AppSettings:IsShowcaseMode");

			// Eğer Vitrin modundaysak VE linkin sonunda ?auto=visitor yazıyorsa
			if (isShowcase && auto == "visitor")
			{
				// Standart "User" hesabını bul
				var user = await _userManager.FindByEmailAsync("user@curelogix.com");

				if (user != null)
				{
					// Şifre sormadan (Bypass) içeri al
					await _signInManager.SignInAsync(user, isPersistent: false);

					// Log at (Hata verirse yut, akış bozulmasın)
					try
					{
						_auditService.TAdd(new Entity.Concrete.AuditLog
						{
							UserName = "Misafir (Auto)",
							Activity = "Portal üzerinden otomatik ziyaretçi girişi.",
							Date = DateTime.Now,
							IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"
						});
					}
					catch { }

					return RedirectToAction("Index", "Home");
				}
			}
			// ============================================================

			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(string username, string password, string ReturnUrl)
		{
			// 🛡️ 1. GÜVENLİK MATRİSİ: Şifre Çözümleme
			// Canlıda (Render) ise server şifresini, yereldeyse varsayılanı kullanır.
			string secureAdminPass = Environment.GetEnvironmentVariable("LIVE_ADMIN_PASSWORD") ?? "CureLogix123!";

			try
			{
				// 🛡️ 2. SELF-HEALING (KENDİ KENDİNİ ONARMA)
				// Eğer Admin girmeye çalışıyorsa ve DB'de yoksa, yeni şifreyle o an oluşturur.
				if (username == "Admin")
				{
					var existingAdmin = await _userManager.FindByNameAsync("Admin");

					if (existingAdmin == null)
					{
						// Rol kontrolü
						if (!await _roleManager.RoleExistsAsync("Admin"))
						{
							await _roleManager.CreateAsync(new AppRole { Name = "Admin" });
						}

						// Admin nesnesi oluşturma
						var newAdmin = new AppUser
						{
							UserName = "Admin",
							Email = "admin@curelogix.com",
							NameSurname = "Sistem Yöneticisi",
							Title = "Başhekim / Sistem Mimarı",
							EmailConfirmed = true
						};

						// KRİTİK: Canlıda ise gizli şifreyle, yereldeyse CureLogix123! ile oluşturur.
						await _userManager.CreateAsync(newAdmin, secureAdminPass);
						await _userManager.AddToRoleAsync(newAdmin, "Admin");
					}
				}

				// 🛡️ 3. GİRİŞ DENEMESİ (İster Admin, İster User/Demo)
				// password; kullanıcı tarafından girilen veridir.
				var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

				if (result.Succeeded)
				{
					// Başarılı Giriş Logu
					try
					{
						_auditService.TAdd(new AuditLog
						{
							UserName = username,
							Activity = "Sisteme giriş yapıldı.",
							Date = DateTime.Now,
							IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"
						});
					}
					catch { }

					if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
						return Redirect(ReturnUrl);

					return RedirectToAction("Index", "Home");
				}
				else
				{
					ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
					return View();
				}
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