using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [AllowAnonymous] // DİKKAT: Global kilidi sadece bu controller için kaldırıyoruz!
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            // ============================================================
            // 🛠️ TOHUM VERİ (SEED DATA) - AKILLI KURULUM
            // Eğer sistemde hiç kullanıcı yoksa (yeni PC, yeni DB), Admin'i oluştur.
            // ============================================================
            var adminUser = await _userManager.FindByNameAsync("Admin");
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = "Admin",
                    Email = "admin@curelogix.com",
                    NameSurname = "Sistem Yöneticisi"
                };

                // Şifre: CureLogix123! (Büyük, küçük, sayı, özel karakter var)
                var createResult = await _userManager.CreateAsync(newAdmin, "CureLogix123!");

                if (!createResult.Succeeded)
                {
                    // Eğer şifre politikasına uymazsa burada hata verir, loglayabiliriz.
                    ViewBag.Error = "Otomatik kurulum hatası: " + string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return View();
                }
            }

            // ============================================================
            // 🔐 GİRİŞ İŞLEMİ
            // ============================================================
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home"); // Başarılı, Dashboard'a git
            }
            else
            {
                ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
        }

        // Çıkış Yapma
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}