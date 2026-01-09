using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.DoctorDTOs;
using CureLogix.Entity.Enums; // Enum için
using CureLogix.WebUI.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CureLogix.WebUI.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IHospitalService _hospitalService;
        private readonly IMapper _mapper;

        public DoctorController(IDoctorService doctorService, IHospitalService hospitalService, IMapper mapper)
        {
            _doctorService = doctorService;
            _hospitalService = hospitalService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            // İleride buraya Include ekleyeceğiz, şimdilik veriyi çekelim
            var values = _doctorService.TGetList();
            var list = _mapper.Map<List<DoctorListDto>>(values);
            return View(list);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // 1. Boş bir ViewModel oluştur
            var model = new DoctorAddViewModel();

            // 2. Hastane Listesini Doldur
            // SelectList yerine SelectListItem listesi yapıyoruz, daha esnek.
            model.HospitalList = _hospitalService.TGetList()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

            // 3. Rol (Enum) Listesini Doldur (İşte Validasyon Hatanı Çözen Kısım)
            // Enum'ı alıp hem "Yazı"sını (Text) hem "Sayı"sını (Value) net olarak belirtiyoruz.
            model.RoleList = Enum.GetValues(typeof(RoleTypeEnum))
                .Cast<RoleTypeEnum>()
                .Select(r => new SelectListItem
                {
                    Text = r.ToString(),      // Örn: "SahaDoktoru"
                    Value = ((int)r).ToString() // Örn: "1" (Artık veritabanına sayı gidecek!)
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(DoctorAddViewModel model)
        {
            // Validasyon artık DTO (model.Data) üzerinden yapılıyor
            DoctorValidator validator = new DoctorValidator();
            ValidationResult results = validator.Validate(model.Data);

            if (results.IsValid)
            {
                // DTO -> Entity Dönüşümü
                var doctor = _mapper.Map<Doctor>(model.Data);

                _doctorService.TAdd(doctor);

                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    // Hata mesajını doğru inputun altına yazdırmak için "Data." öneki ekliyoruz
                    ModelState.AddModelError($"Data.{item.PropertyName}", item.ErrorMessage);
                }
            }

            // HATA DURUMUNDA: Listeleri TEKRAR doldurmak zorundayız (Çünkü HTTP stateless'tır)
            model.HospitalList = _hospitalService.TGetList()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            model.RoleList = Enum.GetValues(typeof(RoleTypeEnum))
                .Cast<RoleTypeEnum>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() }).ToList();

            return View(model);
        }
    }
}