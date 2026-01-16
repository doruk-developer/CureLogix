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