using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.DiseaseDTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class DiseaseController : Controller
    {
        private readonly IDiseaseService _diseaseService;
        private readonly IMapper _mapper;

        public DiseaseController(IDiseaseService diseaseService, IMapper mapper)
        {
            _diseaseService = diseaseService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var values = _diseaseService.TGetList();
            var list = _mapper.Map<List<DiseaseListDto>>(values);
            return View(list);
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
        public IActionResult Add(DiseaseAddDto p)
        {
            DiseaseValidator validator = new DiseaseValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                var disease = _mapper.Map<Disease>(p);

                // --- DÜZELTİLEN KISIM ---
                // Entity string bekliyor, DTO'dan sayı geliyor.
                // Önce null kontrolü yapıyoruz (?? 1), sonra String'e çeviriyoruz.
                disease.RiskLevel = (p.RiskLevel ?? 1).ToString();

                _diseaseService.TAdd(disease);

                TempData["Success"] = "Yeni hastalık tanımı başarıyla yapıldı.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                TempData["Error"] = "Kayıt oluşturulamadı. Lütfen formu kontrol ediniz.";
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
            var value = _diseaseService.TGetById(id);
            if (value == null) return RedirectToAction("Index");

            var updateDto = _mapper.Map<DiseaseUpdateDto>(value);
            return View(updateDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(DiseaseUpdateDto p)
        {
            DiseaseUpdateValidator validator = new DiseaseUpdateValidator();
            ValidationResult results = validator.Validate(p);

            if (!results.IsValid)
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                TempData["Error"] = "Güncelleme başarısız. Girdiğiniz bilgileri kontrol ediniz.";
                return View(p);
            }

            var existingDisease = _diseaseService.TGetById(p.Id);
            if (existingDisease != null)
            {
                // GÜVENLİ MANUEL ATAMALAR (Null Coalescing ??)

                existingDisease.Name = p.Name ?? string.Empty;
                existingDisease.Code = p.Code ?? string.Empty;
                existingDisease.Description = p.Description ?? string.Empty;

                // --- DÜZELTİLEN KISIM ---
                // Sayıyı String'e çevirerek atıyoruz.
                existingDisease.RiskLevel = (p.RiskLevel ?? 1).ToString();

                _diseaseService.TUpdate(existingDisease);

                TempData["Success"] = "Hastalık bilgileri başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        // ==========================================
        // 3. SİLME İŞLEMİ
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var value = _diseaseService.TGetById(id);
            if (value != null)
            {
                _diseaseService.TDelete(value);
                TempData["Success"] = "Hastalık kaydı başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Silinecek kayıt bulunamadı.";
            }
            return RedirectToAction("Index");
        }
    }
}