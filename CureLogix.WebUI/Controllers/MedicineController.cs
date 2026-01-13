using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.MedicineDTOs;
using CureLogix.WebUI.Filters; // AuditLog için
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class MedicineController : Controller
    {
        private readonly IMedicineService _medicineService;
        private readonly IMapper _mapper;

        public MedicineController(IMedicineService medicineService, IMapper mapper)
        {
            _medicineService = medicineService;
            _mapper = mapper;
        }

        // 1. INDEX: Sadece sayfayı yükler (Veriyi AJAX çekecek)
        public IActionResult Index()
        {
            return View();
        }

        // 2. SERVER-SIDE MOTORU (Yeni Eklenen Kısım)
        [HttpPost]
        public IActionResult GetAllMedicinesAjax()
        {
            try
            {
                // DataTables parametrelerini yakala
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Sorguyu Hazırla (Henüz veritabanına gitmedi)
                // DİKKAT: IMedicineService'de GetQuery() metodunun tanımlı olması lazım.
                // Eğer hata verirse GenericRepository ve IGenericService'e GetQuery() eklediğinden emin ol.
                var query = _medicineService.GetQuery();

                // A) FİLTRELEME
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(m =>
                        m.Name.Contains(searchValue) ||
                        m.ActiveIngredient.Contains(searchValue) ||
                        m.Unit.Contains(searchValue));
                }

                // Toplam kayıt sayısı (Filtreden sonra)
                recordsTotal = query.Count();

                // B) SIRALAMA
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    switch (sortColumn)
                    {
                        case "Name":
                            query = sortColumnDirection == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                            break;
                        case "ActiveIngredient":
                            query = sortColumnDirection == "asc" ? query.OrderBy(x => x.ActiveIngredient) : query.OrderByDescending(x => x.ActiveIngredient);
                            break;
                        case "CriticalStockLevel":
                            query = sortColumnDirection == "asc" ? query.OrderBy(x => x.CriticalStockLevel) : query.OrderByDescending(x => x.CriticalStockLevel);
                            break;
                        default:
                            query = query.OrderByDescending(x => x.Id);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(x => x.Id);
                }

                // C) SAYFALAMA VE VERİYİ ÇEKME (SQL Burada Çalışır)
                var data = query.Skip(skip).Take(pageSize).ToList();

                // DTO'ya Çevir
                var dtoList = _mapper.Map<List<MedicineListDto>>(data);

                // JSON Döndür
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = dtoList });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 3. EKLEME İŞLEMİ (Mevcut Kodun)
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [AuditLog("İlaç Kaydı Eklendi")]
        public IActionResult Add(MedicineAddDto p)
        {
            MedicineValidator validator = new MedicineValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                var medicine = _mapper.Map<Medicine>(p);
                _medicineService.TAdd(medicine);
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

        // 4. SİLME İŞLEMİ (Mevcut Kodun)
        [HttpGet]
        [AuditLog("İlaç Kaydı Silindi")]
        public IActionResult Delete(int id)
        {
            var value = _medicineService.TGetById(id);
            _medicineService.TDelete(value);
            return RedirectToAction("Index");
        }
    }
}