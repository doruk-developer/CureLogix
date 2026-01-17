using AutoMapper;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.UserDTOs;
using CureLogix.WebUI.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CureLogix.WebUI.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public UserController(UserManager<AppUser> userManager, IMapper mapper, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _mapper = mapper;
            _env = env;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new UserAddViewModel();
            model.TitleList = GetTitles();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserAddViewModel model)
        {
            AppUserValidator validator = new AppUserValidator();
            ValidationResult results = validator.Validate(model.Data);

            if (results.IsValid)
            {
                var user = _mapper.Map<AppUser>(model.Data);

                // --- FOTOĞRAF YÜKLEME ---
                if (model.Data.Photo != null)
                {
                    var resource = Directory.GetCurrentDirectory();
                    var extension = Path.GetExtension(model.Data.Photo.FileName);
                    var imageName = Guid.NewGuid() + extension;
                    var folderPath = Path.Combine(resource, "wwwroot/uploads/profiles");

                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    var saveLocation = Path.Combine(folderPath, imageName);

                    using (var stream = new FileStream(saveLocation, FileMode.Create))
                    {
                        await model.Data.Photo.CopyToAsync(stream);
                    }
                    user.ProfilePicture = imageName;
                }

                // --- IDENTITY KAYIT ---
                // DÜZELTME BURADA: model.Data.Password'un sonuna '!' koyduk.
                // Bu, "Ben kefilim, burası null gelmeyecek" demektir.
                var result = await _userManager.CreateAsync(user, model.Data.Password!);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError($"Data.{item.PropertyName}", item.ErrorMessage);
                }
            }

            model.TitleList = GetTitles();
            return View(model);
        }

        // -----------------------------------------------------------
        // GÜNCELLEME VE SÜRÜKLE-BIRAK (DROPZONE) İŞLEMLERİ
        // -----------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            // Admin kendini buradan düzenleyemez (Güvenlik)
            if (user == null || user.UserName == "Admin") return RedirectToAction("Index");

            var updateDto = _mapper.Map<UserUpdateDto>(user);

            // Ünvan listesi için ViewBag veya ViewModel kullanabiliriz.
            // Pratik olsun diye ViewBag ile gönderiyoruz (veya UserAddViewModel benzeri bir UpdateViewModel yapabilirsin)
            ViewBag.Titles = GetTitles();

            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto p)
        {
            // Validasyon (Manual Validator çağrısı)
            UserUpdateValidator validator = new UserUpdateValidator();
            ValidationResult results = validator.Validate(p);

            if (!results.IsValid)
            {
                foreach (var item in results.Errors) ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                ViewBag.Titles = GetTitles();
                return View(p);
            }

            var user = await _userManager.FindByIdAsync(p.Id.ToString());
            if (user != null)
            {
                user.Title = p.Title;
                user.NameSurname = p.NameSurname;
                user.UserName = p.Username;
                user.Email = p.Email;

                // Eğer yeni resim yüklendiyse güncelle
                if (!string.IsNullOrEmpty(p.ProfilePicture))
                {
                    user.ProfilePicture = p.ProfilePicture;
                }

                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }

        // DROPZONE İÇİN AJAX RESİM YÜKLEME METODU
        [HttpPost]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Dosya yok" });

            var resource = Directory.GetCurrentDirectory();
            var extension = Path.GetExtension(file.FileName);
            var imageName = Guid.NewGuid() + extension;
            var saveLocation = Path.Combine(resource, "wwwroot/uploads/profiles", imageName);

            using (var stream = new FileStream(saveLocation, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Geriye sadece dosya adını dönüyoruz, formdaki hidden input'a yazacağız.
            return Json(new { success = true, filename = imageName });
        }

        // Kullanıcı silme bloku
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                // GÜVENLİK KONTROLÜ: Admin silinemez!
                if (user.UserName == "Admin")
                {
                    // Hata mesajı ile listeye dön (TempData ile uyarı verilebilir)
                    return RedirectToAction("Index");
                }

                // Kullanıcıyı ve ilişkili verileri siler
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetTitles()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Dr.", Value = "Dr." },
                new SelectListItem { Text = "Uzm. Dr.", Value = "Uzm. Dr." },
                new SelectListItem { Text = "Doç. Dr.", Value = "Doç. Dr." },
                new SelectListItem { Text = "Prof. Dr.", Value = "Prof. Dr." },
                new SelectListItem { Text = "Ord. Prof. Dr.", Value = "Ord. Prof. Dr." },
                new SelectListItem { Text = "Başhemşire", Value = "Başhemşire" },
                new SelectListItem { Text = "Personel", Value = "Personel" },
                new SelectListItem { Text = "Sistem Yöneticisi", Value = "Yönetici" }
            };
        }
    }
}