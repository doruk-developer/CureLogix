using CureLogix.Entity.Concrete;
using CureLogix.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [Authorize] // Bu sayfaya sadece giriş yapmış personel erişebilir
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SettingsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // SAYFA YÜKLENDİĞİNDE (GET)
        [HttpGet]
        public IActionResult Index()
        {
            // Model oluştur ve Cookie'deki tercihleri oku
            // Eğer Cookie yoksa varsayılan değerleri ata
            var model = new SettingsViewModel
            {
                ActiveTheme = Request.Cookies["ThemePreference"] ?? "light",
                ActiveChart = Request.Cookies["ChartPreference"] ?? "bar",
                ActiveSidebarColor = Request.Cookies["SidebarColorPreference"] ?? "primary"
            };

            return View(model);
        }

        // ŞİFRE DEĞİŞTİRME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> ChangePassword(SettingsViewModel model)
        {
            // 1. Form Validasyonu (Boş alan var mı?)
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen form alanlarını eksiksiz ve doğru doldurunuz.";
                // Hata durumunda ayarları kaybetmemek için modeli onarıyoruz
                return View("Index", BuildModelWithCookies(model));
            }

            // 2. Kullanıcıyı Bul
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Login");

            // 3. Identity ile Şifre Değiştirme
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // Kritik: Şifre değişince "SecurityStamp" değişir, oturum düşer.
                // Bu satır oturumu tazeler ve kullanıcının çıkış yapmasını engeller.
                await _signInManager.RefreshSignInAsync(user);

                TempData["Success"] = "Şifreniz başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            else
            {
                // Identity'den gelen hataları (Örn: Eski şifre yanlış) ekrana bas
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    TempData["Error"] = error.Description;
                }

                return View("Index", BuildModelWithCookies(model));
            }
        }

        // TERCİHLERİ KAYDETME (AJAX POST)
        // Bu metot sayfa yenilenmeden JavaScript tarafından çağrılır.
        [HttpPost]
        public IActionResult SetPreference(string theme, string chart, string sidebarColor)
        {
            // Çerez ayarları (1 Yıl geçerli, tüm sitede erişilebilir)
            var options = new CookieOptions { Expires = DateTime.Now.AddYears(1), Path = "/" };

            // Hangi veri geldiyse onu kaydet
            if (!string.IsNullOrEmpty(theme))
                Response.Cookies.Append("ThemePreference", theme, options);

            if (!string.IsNullOrEmpty(chart))
                Response.Cookies.Append("ChartPreference", chart, options);

            if (!string.IsNullOrEmpty(sidebarColor))
                Response.Cookies.Append("SidebarColorPreference", sidebarColor, options);

            return Ok();
        }

        // YARDIMCI METOT (HELPER)
        // Hata durumunda sayfa geri dönerken, kullanıcının renk ayarlarının
        // sıfırlanmasını (null gelmesini) engeller.
        private SettingsViewModel BuildModelWithCookies(SettingsViewModel model)
        {
            model.ActiveTheme = Request.Cookies["ThemePreference"] ?? "light";
            model.ActiveChart = Request.Cookies["ChartPreference"] ?? "bar";
            model.ActiveSidebarColor = Request.Cookies["SidebarColorPreference"] ?? "primary";
            return model;
        }
    }
}