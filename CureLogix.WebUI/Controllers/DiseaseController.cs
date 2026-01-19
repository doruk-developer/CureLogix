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

        // --- EKLEME (ADD) ---
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
                _diseaseService.TAdd(disease);
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
            }
            return View(p);
        }

        // Hastalık güncelleme bilgilerini(formunu) getir
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var value = _diseaseService.TGetById(id);
            if (value == null) return RedirectToAction("Index");

            // Formu doldurmak için Entity -> DTO dönüşümü
            var updateDto = _mapper.Map<DiseaseUpdateDto>(value);
            return View(updateDto);
        }

        // Hastalık güncelleme verilerini sisteme gönder(kaydet)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(DiseaseUpdateDto p)
        {
            // 1. Validasyon
            DiseaseUpdateValidator validator = new DiseaseUpdateValidator();
            ValidationResult results = validator.Validate(p);

            if (!results.IsValid)
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                return View(p);
            }

            // 2. GÜVENLİ GÜNCELLEME (Fetch-Map-Save)
            var existingDisease = _diseaseService.TGetById(p.Id);

            if (existingDisease != null)
            {
                // Manuel Eşleme (Hata riskini sıfırlar)
                existingDisease.Name = p.Name;
                existingDisease.Code = p.Code;
                existingDisease.Description = p.Description;
                existingDisease.RiskLevel = p.RiskLevel;

                _diseaseService.TUpdate(existingDisease);
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        // --- SİLME (DELETE) ---
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var value = _diseaseService.TGetById(id);
            if (value != null)
            {
                _diseaseService.TDelete(value);
            }
            return RedirectToAction("Index");
        }
    }
}