using CureLogix.Entity.Concrete;
using CureLogix.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [Authorize] // Sadece giriş yapanlar erişebilir
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SettingsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Cookie'den mevcut tercihleri okuyoruz, yoksa varsayılan atıyoruz
            var model = new SettingsViewModel
            {
                ActiveTheme = Request.Cookies["ThemePreference"] ?? "light",
                ActiveChart = Request.Cookies["ChartPreference"] ?? "bar"
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(SettingsViewModel model)
        {
            // 1. Model Validasyonu (Boş alan kontrolü)
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları kurallara uygun doldurunuz.";
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Login");

            // 2. Identity ile Şifre Değiştirme
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // 3. Başarılıysa: Oturumu Tazele ve Bilgi Ver
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Şifreniz başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            else
            {
                // 4. Başarısızsa: Hataları Topla ve Göster
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    // Hataları ekranda kırmızı kutuda göstermek için:
                    TempData["Error"] = error.Description;
                }

                // Ayarları tekrar yükle ki sayfa bozuk görünmesin
                ViewBag.CurrentTheme = Request.Cookies["ThemePreference"] ?? "light";
                ViewBag.ChartType = Request.Cookies["ChartPreference"] ?? "bar";

                return View("Index", model);
            }
        }

        [HttpPost]
        public IActionResult SetPreference(string theme, string chart)
        {
            // Tercihleri Cookie'ye yazıyoruz (1 Yıl Geçerli)
            var options = new CookieOptions { Expires = DateTime.Now.AddYears(1), Path = "/" };

            if (!string.IsNullOrEmpty(theme)) Response.Cookies.Append("ThemePreference", theme, options);
            if (!string.IsNullOrEmpty(chart)) Response.Cookies.Append("ChartPreference", chart, options);

            return Ok();
        }
    }
}