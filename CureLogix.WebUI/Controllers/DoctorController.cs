using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.DoctorDTOs;
using CureLogix.Entity.Enums;
using CureLogix.WebUI.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
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
            var values = _doctorService.TGetList();
            var list = _mapper.Map<List<DoctorListDto>>(values);
            return View(list);
        }

        // --- EKLEME İŞLEMLERİ ---
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            var model = new DoctorAddViewModel();
            FillDropdowns(model);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(DoctorAddViewModel model)
        {
            DoctorValidator validator = new DoctorValidator();
            ValidationResult results = validator.Validate(model.Data);

            if (results.IsValid)
            {
                var doctor = _mapper.Map<Doctor>(model.Data);
                _doctorService.TAdd(doctor);
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError($"Data.{item.PropertyName}", item.ErrorMessage);
                }
            }

            FillDropdowns(model);
            return View(model);
        }

        // --- GÜNCELLEME İŞLEMLERİ ---
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var entity = _doctorService.TGetById(id);
            if (entity == null) return RedirectToAction("Index");

            // Entity -> DTO Dönüşümü
            // Burada RoleType int olarak geldiği için sorun yok.
            var updateDto = _mapper.Map<DoctorUpdateDto>(entity);

            var model = new DoctorUpdateViewModel
            {
                Data = updateDto
            };

            FillDropdownsForUpdate(model);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(DoctorUpdateViewModel model)
        {
            // 1. Validasyon
            DoctorUpdateValidator validator = new DoctorUpdateValidator();
            ValidationResult results = validator.Validate(model.Data);

            if (!results.IsValid)
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError($"Data.{item.PropertyName}", item.ErrorMessage);
                }

                // Hata varsa dropdownları yeniden doldur
                FillDropdownsForUpdate(model);
                return View(model);
            }

            // 2. İşlem Başarılıysa Kaydet
            var existingDoctor = _doctorService.TGetById(model.Data.Id);

            if (existingDoctor != null)
            {
                // Manuel Eşleştirme (En Güvenlisi)
                existingDoctor.FullName = model.Data.FullName;
                existingDoctor.Title = model.Data.Title;
                existingDoctor.Specialty = model.Data.Specialty;

                // KRİTİK: RoleType burada doğru geliyor çünkü Validasyon geçti
                existingDoctor.RoleType = model.Data.RoleType;

                existingDoctor.HospitalId = model.Data.HospitalId;
                existingDoctor.Email = model.Data.Email;

                _doctorService.TUpdate(existingDoctor);
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // 1. Silinecek kaydı bul
            var value = _doctorService.TGetById(id);

            // 2. Kayıt varsa sil
            if (value != null)
            {
                _doctorService.TDelete(value);
            }

            // 3. Listeye geri dön
            return RedirectToAction("Index");
        }


        // --- YARDIMCI METOTLAR ---

        private void FillDropdowns(DoctorAddViewModel model)
        {
            model.HospitalList = _hospitalService.TGetList()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            model.RoleList = Enum.GetValues(typeof(RoleTypeEnum))
                .Cast<RoleTypeEnum>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() }).ToList();
        }

        // GÜNCELLEME İÇİN DROPDOWN DOLDURMA
        private void FillDropdownsForUpdate(DoctorUpdateViewModel model)
        {
            // 1. Hastaneleri Getir
            model.HospitalList = _hospitalService.TGetList()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

            // 2. Rolleri (Enum) MANUEL ve GARANTİ Yöntemle Getir
            // Reflection kullanmadan, tek tek ekliyoruz ki "Value" kesinlikle sayı olsun.
            model.RoleList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Saha Doktoru", Value = "1" },
                new SelectListItem { Text = "Konsey Üyesi", Value = "2" },
                new SelectListItem { Text = "Başhekim", Value = "3" },
                new SelectListItem { Text = "Bakanlık Yetkilisi", Value = "4" }
            };
        }
    }
}