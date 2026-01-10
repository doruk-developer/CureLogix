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
        private readonly IQrCodeService _qrService; // QR Servisi

        // TEK VE TAM CONSTRUCTOR (Tüm servisler burada)
        public CentralStockController(
            ICentralWarehouseService warehouseService,
            IMedicineService medicineService,
            IMapper mapper,
            IQrCodeService qrService)
        {
            _warehouseService = warehouseService;
            _medicineService = medicineService;
            _mapper = mapper;
            _qrService = qrService;
        }

        // 1. STOK LİSTELEME
        public IActionResult Index()
        {
            var values = _warehouseService.TGetList();
            var list = _mapper.Map<List<CentralStockListDto>>(values);
            return View(list);
        }

        // 2. STOK GİRİŞ SAYFASI (GET)
        [HttpGet]
        public IActionResult Entry()
        {
            var model = new CentralStockViewModel();
            model.Data.ExpiryDate = DateTime.Today.AddYears(1);

            model.MedicineList = _medicineService.TGetList()
                .Select(x => new SelectListItem
                {
                    Text = $"{x.Name} ({x.Unit})",
                    Value = x.Id.ToString()
                }).ToList();

            return View(model);
        }

        // 3. STOK GİRİŞ İŞLEMİ (POST)
        [HttpPost]
        public IActionResult Entry(CentralStockViewModel model)
        {
            CentralStockValidator validator = new CentralStockValidator();
            ValidationResult results = validator.Validate(model.Data);

            if (results.IsValid)
            {
                var stock = _mapper.Map<CentralWarehouse>(model.Data);
                stock.ManufacturingDate = DateTime.Now;

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

            model.MedicineList = _medicineService.TGetList()
                .Select(x => new SelectListItem { Text = $"{x.Name} ({x.Unit})", Value = x.Id.ToString() }).ToList();

            return View(model);
        }

        // 4. YENİ: QR KOD GÖRÜNTÜLEME (Resim Olarak Döner)
        public IActionResult GetQrImage(int id)
        {
            var stock = _warehouseService.TGetById(id);
            if (stock == null) return NotFound();

            // Format: STOK-{ID}-{BATCH}
            string qrContent = $"STOK-{stock.Id}-{stock.BatchNo}";

            var image = _qrService.GenerateQrCode(qrContent);

            return File(image, "image/png");
        }

        // 5. YENİ: BARKOD OKUYUCU EKRANI (GET)
        [HttpGet]
        public IActionResult Scan()
        {
            return View();
        }

        // 6. YENİ: OKUTULAN KODU İŞLEME (POST)
        [HttpPost]
        public IActionResult Scan(string qrData, int quantity)
        {
            try
            {
                // Gelen veri formatı: STOK-12-BATCH01
                if (string.IsNullOrEmpty(qrData) || !qrData.StartsWith("STOK-"))
                {
                    TempData["Error"] = "Geçersiz QR Kod formatı!";
                    return View();
                }

                // ID'yi ayıkla (Split ile parçala)
                var parts = qrData.Split('-');
                int stockId = int.Parse(parts[1]);

                var stock = _warehouseService.TGetById(stockId);
                if (stock == null)
                {
                    TempData["Error"] = "Stok bulunamadı!";
                    return View();
                }

                if (stock.Quantity < quantity)
                {
                    TempData["Error"] = "Yetersiz stok! Mevcut: " + stock.Quantity;
                    return View();
                }

                // Stoktan Düş
                stock.Quantity -= quantity;
                _warehouseService.TUpdate(stock);

                TempData["Success"] = $"✅ {quantity} adet ürün stoktan başarıyla düşüldü. (Kalan: {stock.Quantity})";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return View();
        }
    }
}