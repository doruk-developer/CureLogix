using CureLogix.Business.Abstract; // AuditLogService için
using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditLogService _auditService; // 1. Servisi Ekledik

        public LoginController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IAuditLogService auditService) // 2. Constructor'a ekledik
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditService = auditService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            // Otomatik Admin Oluşturma (Seed Data)
            var adminUser = await _userManager.FindByNameAsync("Admin");
            if (adminUser == null)
            {
                var newAdmin = new AppUser { UserName = "Admin", Email = "admin@curelogix.com", NameSurname = "Sistem Yöneticisi" };
                await _userManager.CreateAsync(newAdmin, "CureLogix123!");
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                // 3. GİRİŞ LOGU (Burada kaydediyoruz)
                _auditService.TAdd(new AuditLog
                {
                    UserName = username,
                    ActionType = "Login",
                    ControllerName = "LoginController",
                    Description = "Kullanıcı sisteme başarılı giriş yaptı.",
                    ProcessDate = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Localhost"
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // İstersen "Hatalı Giriş Denemesi"ni de loglayabilirsin (Güvenlik için süper olur)
                _auditService.TAdd(new AuditLog
                {
                    UserName = username, // Denenen kullanıcı adı
                    ActionType = "Login Failed",
                    ControllerName = "LoginController",
                    Description = "Hatalı şifre veya kullanıcı adı denemesi.",
                    ProcessDate = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Localhost"
                });

                ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            // 4. ÇIKIŞ LOGU (Çıkış yapmadan hemen önce kaydediyoruz)
            var currentUser = User.Identity?.Name ?? "Bilinmeyen";

            _auditService.TAdd(new AuditLog
            {
                UserName = currentUser,
                ActionType = "Logout",
                ControllerName = "LoginController",
                Description = "Kullanıcı güvenli çıkış yaptı.",
                ProcessDate = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Localhost"
            });

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}