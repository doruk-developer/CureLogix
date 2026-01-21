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

        // ==========================================
        // 1. EKLEME İŞLEMİ (ADD)
        // ==========================================
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

                // ✅ BAŞARILI MESAJI
                TempData["Success"] = "Yeni personel kaydı başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }
            else
            {
                // Hataları Model'e ekle
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError($"Data.{item.PropertyName}", item.ErrorMessage);
                }

                // ❌ HATA MESAJI
                TempData["Error"] = "Kayıt eklenemedi. Lütfen formdaki eksik alanları doldurunuz.";
            }

            // Hata durumunda Dropdownları tekrar doldur ki sayfa bozulmasın
            FillDropdowns(model);
            return View(model);
        }

        // ==========================================
        // 2. GÜNCELLEME İŞLEMİ (UPDATE)
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var entity = _doctorService.TGetById(id);
            if (entity == null) return RedirectToAction("Index");

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
            DoctorUpdateValidator validator = new DoctorUpdateValidator();
            ValidationResult results = validator.Validate(model.Data);

            if (!results.IsValid)
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError($"Data.{item.PropertyName}", item.ErrorMessage);
                }

                TempData["Error"] = "Güncelleme başarısız. Lütfen bilgileri kontrol ediniz.";

                FillDropdownsForUpdate(model);
                return View(model);
            }

            var existingDoctor = _doctorService.TGetById(model.Data.Id);

            if (existingDoctor != null)
            {
                // GÜVENLİ ATAMALAR (Null Gelirse Boş String veya 0 Ata)

                existingDoctor.FullName = model.Data.FullName ?? string.Empty;
                existingDoctor.Title = model.Data.Title ?? string.Empty;
                existingDoctor.Specialty = model.Data.Specialty ?? string.Empty;

                // Sayı null ise 0 yap
                existingDoctor.RoleType = model.Data.RoleType ?? 0;

                existingDoctor.HospitalId = model.Data.HospitalId;

                // --- DÜZELTİLEN SON SATIR ---
                // Email null ise veritabanına boş metin ("") kaydet.
                existingDoctor.Email = model.Data.Email ?? string.Empty;

                _doctorService.TUpdate(existingDoctor);

                TempData["Success"] = "Personel bilgileri başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        // ==========================================
        // YARDIMCI METOTLAR
        // ==========================================

        private void FillDropdowns(DoctorAddViewModel model)
        {
            model.HospitalList = _hospitalService.TGetList()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            model.RoleList = Enum.GetValues(typeof(RoleTypeEnum))
                .Cast<RoleTypeEnum>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() }).ToList();
        }

        private void FillDropdownsForUpdate(DoctorUpdateViewModel model)
        {
            model.HospitalList = _hospitalService.TGetList()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

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