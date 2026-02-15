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
        public IActionResult Index(string ReturnUrl)
        {
			// Eğer kullanıcı zaten içerideyse, tekrar login sayfasına gelmesin
			if (User.Identity?.IsAuthenticated == true)
			{
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password, string ReturnUrl)
        {
			// ============================================================
			// 1. DEMO MODU KONTROLÜ (SATIŞ SUNUMU İÇİN)
			// ============================================================
			// 🛡️ KRİTİK: Şifreyi sunucu kasasından oku, yoksa yerel varsayılanı (CureLogix123!) kullan.
			// Bu sayede GitHub'daki şifre canlıda bir işe yaramaz hale gelir.
			string secureAdminPass = Environment.GetEnvironmentVariable("LIVE_ADMIN_PASSWORD") ?? "CureLogix123!";

			bool isDemo = _configuration.GetValue<bool>("AppSettings:DemoMode");

			if (isDemo)
			{
				// Demo modundaysak ve şifre doğruysa DB'ye sormadan içeri al (Bypass)
				if (username == "Admin" && password == secureAdminPass)
				{
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, username),
						new Claim(ClaimTypes.Role, "Admin")
					};

					var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
					var authProperties = new AuthenticationProperties { IsPersistent = true };

					await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

					return RedirectToAction("Index", "Home");
				}
				else
				{
					ViewBag.Error = "Demo Modu: Kullanıcı adı veya şifre hatalı!";
					return View();
				}
			}

			// ============================================================
			// 2. NORMAL MOD (VERİTABANI BAĞLANTISI)
			// ============================================================
			try
            {
                // --- SELF-HEALING (KENDİ KENDİNİ ONARMA) ---
                // Veritabanı sıfırlandıysa Admin kullanıcısı silinmiştir.
                // Eğer "Admin" girmeye çalışıyorsa ve yoksa, o an oluştur.
                if (username == "Admin")
                {
                    var existingAdmin = await _userManager.FindByNameAsync("Admin");

                    if (existingAdmin == null)
                    {
                        // 1. Önce Rolü Kontrol Et/Oluştur
                        if (!await _roleManager.RoleExistsAsync("Admin"))
                        {
                            await _roleManager.CreateAsync(new AppRole { Name = "Admin" });
                        }

                        // 2. Kullanıcıyı Oluştur
                        var newAdmin = new AppUser
                        {
                            UserName = "Admin",
                            Email = "admin@curelogix.com",
                            NameSurname = "Sistem Yöneticisi",
                            Title = "Başhekim",
                            EmailConfirmed = true,
                            ProfilePicture = ""
                        };

						// Admin kullanıcısı yoksa sıfırdan oluştururken kullanılan şifreyi de aynı yöntemle koruyoruz.
						var createResult = await _userManager.CreateAsync(newAdmin, secureAdminPass);

						// 3. Rolü Ata
						if (createResult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(newAdmin, "Admin");
                        }
                    }
                }
                // -------------------------------------------------------------

                // GİRİŞ DENEMESİ
                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

                if (result.Succeeded)
                {
                    // ✅ BAŞARILI GİRİŞ LOGU (Yeni Yapı)
                    // Loglama başarısız olursa (DB sorunu vb.) giriş işlemini engellemesin diye try-catch
                    try
                    {
                        _auditService.TAdd(new AuditLog
                        {
                            UserName = username,
                            Activity = "Kullanıcı sisteme başarılı giriş yaptı.",
                            Date = DateTime.Now,
                            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"
                        });
                    }
                    catch { /* Log atılamadı, sessizce devam et */ }

                    // Eğer bir sayfadan yönlendirilmişse oraya dön, yoksa Ana Sayfaya git
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        return Redirect(ReturnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    // ❌ BAŞARISIZ GİRİŞ LOGU
                    try
                    {
                        _auditService.TAdd(new AuditLog
                        {
                            UserName = username,
                            Activity = "Hatalı şifre veya kullanıcı adı denemesi.",
                            Date = DateTime.Now,
                            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1"
                        });
                    }
                    catch { /* Sessiz kal */ }

                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Veritabanı bağlantısı kopuksa veya başka kritik hata varsa
                ViewBag.Error = "Sistem Hatası (DB Bağlantısı Yok): " + ex.Message;
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