using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.WarehouseDTOs;
using CureLogix.WebUI.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CureLogix.WebUI.Controllers
{
    public class CentralStockController : Controller
    {
        private readonly ICentralWarehouseService _warehouseService;
        private readonly IMedicineService _medicineService;
        private readonly IMapper _mapper;

        public CentralStockController(ICentralWarehouseService warehouseService, IMedicineService medicineService, IMapper mapper)
        {
            _warehouseService = warehouseService;
            _medicineService = medicineService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var values = _warehouseService.TGetList();
            var list = _mapper.Map<List<CentralStockListDto>>(values);
            return View(list);
        }

        [HttpGet]
        public IActionResult Entry() // Add yerine Entry (Stok Giriş) diyelim
        {
            var model = new CentralStockViewModel();
            model.Data.ExpiryDate = DateTime.Today.AddYears(1); // Varsayılan tarih 1 yıl sonra

            // Dropdown Doldurma
            model.MedicineList = _medicineService.TGetList()
                .Select(x => new SelectListItem
                {
                    Text = $"{x.Name} ({x.Unit})",
                    Value = x.Id.ToString()
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Entry(CentralStockViewModel model)
        {
            CentralStockValidator validator = new CentralStockValidator();
            ValidationResult results = validator.Validate(model.Data);

            if (results.IsValid)
            {
                var stock = _mapper.Map<CentralWarehouse>(model.Data);
                stock.ManufacturingDate = DateTime.Now; // Giriş tarihi = Üretim/İşlem tarihi varsayalım

                _warehouseService.TAdd(stock);
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError($"Data.{item.PropertyName}", item.ErrorMessage);
                }
            }

            // Hata varsa listeyi tekrar doldur
            model.MedicineList = _medicineService.TGetList()
                .Select(x => new SelectListItem { Text = $"{x.Name} ({x.Unit})", Value = x.Id.ToString() }).ToList();

            return View(model);
        }
    }
}