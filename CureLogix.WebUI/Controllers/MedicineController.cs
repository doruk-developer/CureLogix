using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.MedicineDTOs;
using CureLogix.WebUI.Filters; // AuditLog için
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Index()
        {
            return View();
        }

        // ==========================================
        // 1. SERVER-SIDE DATATABLES (AJAX MOTORU)
        // ==========================================
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

                // Sorguyu Hazırla
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

                // C) SAYFALAMA VE VERİYİ ÇEKME
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

        // ==========================================
        // 2. EKLEME İŞLEMİ (ADD)
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            // Boş bir DTO göndererek View'ın null hatası vermesini engelliyoruz.
            // DTO'daki 'bool RequiresColdChain' artık varsayılan olarak 'false' gelecek.
            return View(new MedicineAddDto());
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

                // GÜVENLİ ATAMALAR (Null gelirse varsayılan değer ata)
                // Bu sayede veritabanı "Not Null" hatası vermez.
                medicine.ShelfLifeDays = p.ShelfLifeDays ?? 0;
                medicine.CriticalStockLevel = p.CriticalStockLevel ?? 0;
                medicine.RequiresColdChain = p.RequiresColdChain;

                _medicineService.TAdd(medicine);

                // ✅ BAŞARILI MESAJI
                TempData["Success"] = "İlaç sisteme başarıyla kaydedildi.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                // ❌ HATA MESAJI
                TempData["Error"] = "Kayıt başarısız. Lütfen bilgileri kontrol ediniz.";
            }
            return View(p);
        }

        // ==========================================
        // 3. GÜNCELLEME İŞLEMİ (UPDATE)
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var value = _medicineService.TGetById(id);
            if (value == null) return RedirectToAction("Index");

            var updateDto = _mapper.Map<MedicineUpdateDto>(value);
            return View(updateDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(MedicineUpdateDto p)
        {
            MedicineUpdateValidator validator = new MedicineUpdateValidator();
            ValidationResult results = validator.Validate(p);

            if (!results.IsValid)
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
                // ❌ HATA MESAJI
                TempData["Error"] = "Güncelleme sırasında hata oluştu. Formu kontrol ediniz.";
                return View(p);
            }

            var existingMedicine = _medicineService.TGetById(p.Id);
            if (existingMedicine != null)
            {
                // GÜVENLİ MANUEL ATAMALAR (Null Coalescing ??)
                // DTO nullable olduğu için, null gelme ihtimaline karşı varsayılan değerleri atıyoruz.

                existingMedicine.Name = p.Name ?? string.Empty;
                existingMedicine.ActiveIngredient = p.ActiveIngredient ?? string.Empty;
                existingMedicine.Unit = p.Unit ?? string.Empty;

                existingMedicine.ShelfLifeDays = p.ShelfLifeDays ?? 0;
                existingMedicine.CriticalStockLevel = p.CriticalStockLevel ?? 0;
                existingMedicine.RequiresColdChain = p.RequiresColdChain;

                _medicineService.TUpdate(existingMedicine);

                // ✅ BAŞARILI MESAJI
                TempData["Success"] = "İlaç detayları başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        // ==========================================
        // 4. SİLME İŞLEMİ (DELETE)
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [AuditLog("İlaç Kaydı Silindi")]
        public IActionResult Delete(int id)
        {
            var value = _medicineService.TGetById(id);
            if (value != null)
            {
                _medicineService.TDelete(value);
                // ✅ BAŞARILI MESAJI
                TempData["Success"] = "İlaç kaydı sistemden silindi.";
            }
            else
            {
                // ❌ HATA MESAJI
                TempData["Error"] = "Silinecek kayıt bulunamadı.";
            }
            return RedirectToAction("Index");
        }
    }
}