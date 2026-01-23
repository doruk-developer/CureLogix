using CureLogix.Business.Abstract; // Log Servisi için
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
        private readonly IAuditLogService _auditService; // YENİ: Log servisi eklendi

        public SettingsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAuditLogService auditService) // YENİ: Constructor'a eklendi
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _auditService = auditService;
        }

        // SAYFA YÜKLENDİĞİNDE (GET)
        [HttpGet]
        public IActionResult Index()
        {
            // 1. Log Geçmişini Çek (Son 5 Kayıt)
            var username = User.Identity?.Name ?? "";

            // Eğer kullanıcı adı yoksa boş liste döndür (Güvenlik)
            var userLogs = new List<AuditLog>();

            if (!string.IsNullOrEmpty(username))
            {
                // Filtreleme: Sadece bu kullanıcının loglarını getir
                // Sıralama: En yeniden eskiye
                // Limitleme: Sadece son 5 kayıt
                userLogs = _auditService.TGetList() // GenericRepository'de GetListByFilter varsa onu kullanmak daha performanslıdır
                                .Where(x => x.UserName == username)
                                .OrderByDescending(x => x.Date)
                                .Take(30)
                                .ToList();
            }

            // 2. Modeli Doldur
            var model = new SettingsViewModel
            {
                ActiveTheme = Request.Cookies["ThemePreference"] ?? "light",
                ActiveChart = Request.Cookies["ChartPreference"] ?? "bar",
                ActiveSidebarColor = Request.Cookies["SidebarColorPreference"] ?? "primary",

                // YENİ: Log listesini modele ekle
                LogHistory = userLogs
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
        [HttpPost]
        public IActionResult SetPreference(string theme, string chart, string sidebarColor)
        {
            var options = new CookieOptions { Expires = DateTime.Now.AddYears(1), Path = "/" };

            if (!string.IsNullOrEmpty(theme))
                Response.Cookies.Append("ThemePreference", theme, options);

            if (!string.IsNullOrEmpty(chart))
                Response.Cookies.Append("ChartPreference", chart, options);

            if (!string.IsNullOrEmpty(sidebarColor))
                Response.Cookies.Append("SidebarColorPreference", sidebarColor, options);

            return Ok();
        }

        // YARDIMCI METOT (HELPER)
        private SettingsViewModel BuildModelWithCookies(SettingsViewModel model)
        {
            model.ActiveTheme = Request.Cookies["ThemePreference"] ?? "light";
            model.ActiveChart = Request.Cookies["ChartPreference"] ?? "bar";
            model.ActiveSidebarColor = Request.Cookies["SidebarColorPreference"] ?? "primary";

            // Hata alıp sayfaya geri dönerken Log tablosu boş kalmasın diye tekrar çekiyoruz
            var username = User.Identity?.Name ?? "";
            if (!string.IsNullOrEmpty(username))
            {
                model.LogHistory = _auditService.TGetList()
                                .Where(x => x.UserName == username)
                                .OrderByDescending(x => x.Date)
                                .Take(30)
                                .ToList();
            }

            return model;
        }
    }
}