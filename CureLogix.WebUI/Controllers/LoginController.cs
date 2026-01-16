using CureLogix.Business.Abstract;
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
        private readonly RoleManager<AppRole> _roleManager; // ROL YÖNETİCİSİ
        private readonly IAuditLogService _auditService;    // LOGLAMA SERVİSİ

        public LoginController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager, // <--- EKLENDİ
            IAuditLogService auditService)    // <--- EKLENDİ
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
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
            // --- 1. OTOMATİK ADMİN KURULUMU (SEED DATA) ---
            var adminUser = await _userManager.FindByNameAsync("Admin");

            if (adminUser == null)
            {
                // A) Rol Yoksa Oluştur
                if (await _roleManager.FindByNameAsync("Admin") == null)
                {
                    await _roleManager.CreateAsync(new AppRole { Name = "Admin" });
                }

                // B) Kullanıcıyı Oluştur
                var newAdmin = new AppUser
                {
                    UserName = "Admin",
                    Email = "admin@curelogix.com",
                    NameSurname = "Sistem Yöneticisi"
                };

                var createResult = await _userManager.CreateAsync(newAdmin, "CureLogix123!");

                // C) Kullanıcıyı Role Ata
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
            // -----------------------------------------------

            // --- 2. GİRİŞ İŞLEMİ ---
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                // GİRİŞ LOGU (BAŞARILI)
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
                // GİRİŞ LOGU (BAŞARISIZ)
                _auditService.TAdd(new AuditLog
                {
                    UserName = username,
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
            // ÇIKIŞ LOGU
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