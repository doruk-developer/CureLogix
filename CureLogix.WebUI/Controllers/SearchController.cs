using CureLogix.Business.Abstract;
using CureLogix.Entity.DTOs.SearchDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly IElasticSearchService _elasticService;
        private readonly IMedicineService _medicineService;

        public SearchController(IElasticSearchService elasticService, IMedicineService medicineService)
        {
            _elasticService = elasticService;
            _medicineService = medicineService;
        }

        [HttpGet]
        public async Task<IActionResult> SyncData()
        {
            try
            {
                await _elasticService.DeleteIndexAsync();
                await _elasticService.CreateIndexAsync();

                var allMedicines = _medicineService.TGetList();

                // 🕵️‍♂️ GÜVENLİK ÖNLEMİ: Eğer veritabanından ID 0 geliyorsa, 
                // biz kendi sayacımızı (counter) kullanalım ki Elastic üst üste yazmasın.
                int counter = 1;
                var searchModels = allMedicines.Select(item => new MedicineSearchModel
                {
                    // Eğer item.Id 0 ise counter'ı kullan, değilse gerçek ID'yi kullan
                    Id = item.Id > 0 ? item.Id : counter++,
                    Name = item.Name ?? "İsimsiz",
                    ActiveIngredient = item.ActiveIngredient ?? "-",
                    Unit = item.Unit ?? "-",
                    StockQuantity = 100,
                    IsCritical = false
                }).ToList();

                await _elasticService.BulkIndexMedicinesAsync(searchModels);

                // 🚀 DETAYLI RAPOR (Tarayıcıda bunu görmelisin)
                return Content("Veriler ElasticSearch'e akratıldı: " + allMedicines.Count + " adet veri aktarıldı");
            }
            catch (Exception ex) { return Content($"❌ HATA: {ex.Message}"); }
        }

        [HttpGet]
        public async Task<IActionResult> QuickSearch(string q)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 2) return Json(new List<MedicineSearchModel>());
            var results = await _elasticService.SearchAsync(q);
            return Json(results);
        }
    }
}