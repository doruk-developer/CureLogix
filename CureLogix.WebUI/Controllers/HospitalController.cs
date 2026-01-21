using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.HospitalDTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly IMapper _mapper;

        public HospitalController(IHospitalService hospitalService, IMapper mapper)
        {
            _hospitalService = hospitalService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var values = _hospitalService.TGetList();
            var hospitalList = _mapper.Map<List<HospitalListDto>>(values);
            return View(hospitalList);
        }

        // ==========================================
        // 1. EKLEME İŞLEMİ
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(HospitalAddDto p)
        {
            HospitalValidator validator = new HospitalValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                var hospitalEntity = _mapper.Map<Hospital>(p);

                // GÜVENLİ ATAMALAR:
                // DTO'dan null gelse bile varsayılan değerler atıyoruz.
                hospitalEntity.MainStorageCapacity = p.MainStorageCapacity ?? 0;
                hospitalEntity.WasteStorageCapacity = p.WasteStorageCapacity ?? 0;

                // Yeni hastane boş ve aktif başlar
                hospitalEntity.OccupancyRate = 0;
                hospitalEntity.IsActive = true;

                _hospitalService.TAdd(hospitalEntity);

                TempData["Success"] = "Yeni hastane başarıyla sisteme tanımlandı.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                TempData["Error"] = "Kayıt başarısız. Lütfen formdaki hataları gideriniz.";
            }

            return View(p);
        }

        // ==========================================
        // 2. GÜNCELLEME İŞLEMİ
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var value = _hospitalService.TGetById(id);
            if (value == null) return RedirectToAction("Index");

            var updateDto = _mapper.Map<HospitalUpdateDto>(value);
            return View(updateDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(HospitalUpdateDto p)
        {
            HospitalUpdateValidator validator = new HospitalUpdateValidator();
            ValidationResult results = validator.Validate(p);

            if (!results.IsValid)
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                TempData["Error"] = "Güncelleme sırasında hata oluştu. Bilgileri kontrol ediniz.";
                return View(p);
            }

            var existingHospital = _hospitalService.TGetById(p.Id);
            if (existingHospital != null)
            {
                // --- GÜVENLİ MANUEL ATAMALAR (Null Gelirse Yedek Değer Kullan) ---

                existingHospital.Name = p.Name ?? string.Empty;
                existingHospital.City = p.City ?? string.Empty;

                // Sayısal değerler null ise 0 yap
                existingHospital.MainStorageCapacity = p.MainStorageCapacity ?? 0;
                existingHospital.WasteStorageCapacity = p.WasteStorageCapacity ?? 0;
                existingHospital.OccupancyRate = p.OccupancyRate ?? 0;

                // Boolean null ise false yap (veya varsayılan true)
                existingHospital.IsActive = p.IsActive ?? false;

                _hospitalService.TUpdate(existingHospital);

                TempData["Success"] = "Hastane bilgileri başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var value = _hospitalService.TGetById(id);
            if (value != null)
            {
                _hospitalService.TDelete(value);
                TempData["Success"] = "Hastane kaydı sistemden başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Silinecek kayıt bulunamadı.";
            }
            return RedirectToAction("Index");
        }
    }
}