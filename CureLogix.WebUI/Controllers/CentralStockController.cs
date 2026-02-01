using AutoMapper;
using ClosedXML.Excel;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.WarehouseDTOs;
using CureLogix.WebUI.Filters;
using CureLogix.WebUI.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

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
        [AuditLog("Merkez Depo Stok Girişi")]
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
        [AuditLog("QR ile Hızlı Stok Çıkışı")]
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

        public IActionResult ExportExcel()
        {
            // 1. Veriyi Çek
            var stocks = _warehouseService.TGetList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Merkez Depo Stokları");

                // --- A. BAŞLIKLAR ---
                worksheet.Cell(1, 1).Value = "İlaç Adı";
                worksheet.Cell(1, 2).Value = "Miktar";
                worksheet.Cell(1, 3).Value = "Birim";
                worksheet.Cell(1, 4).Value = "Parti No (Batch)";
                worksheet.Cell(1, 5).Value = "Son Kullanma Tarihi";
                worksheet.Cell(1, 6).Value = "Durum";

                // --- B. VERİLERİ DÖK ---
                int row = 2;
                foreach (var item in stocks)
                {
                    worksheet.Cell(row, 1).Value = item.Medicine?.Name;

                    // Miktar (Sayı formatı)
                    worksheet.Cell(row, 2).Value = item.Quantity;
                    worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0";

                    worksheet.Cell(row, 3).Value = item.Medicine?.Unit;
                    worksheet.Cell(row, 4).Value = item.BatchNo;
                    worksheet.Cell(row, 5).Value = item.ExpiryDate;

                    // Durum Analizi (Renklendirme)
                    if (item.ExpiryDate < DateTime.Now)
                    {
                        worksheet.Cell(row, 6).Value = "SKT DOLDU";
                        worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(row, 6).Style.Font.Bold = true;
                    }
                    else if (item.Quantity < 100)
                    {
                        worksheet.Cell(row, 6).Value = "KRİTİK";
                        worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Orange;
                    }
                    else
                    {
                        worksheet.Cell(row, 6).Value = "UYGUN";
                        worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Green;
                    }

                    row++;
                }

                // --- C. PROFESYONEL STİL ---
                var tabloAraligi = worksheet.Range(1, 1, row - 1, 6);

                // 1. Hizalama (Tam Ortala)
                tabloAraligi.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                tabloAraligi.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // 2. Başlık Stili (Kurumsal Mavi)
                var baslik = worksheet.Range("A1:F1");
                baslik.Style.Font.Bold = true;
                baslik.Style.Font.FontColor = XLColor.White;
                baslik.Style.Fill.BackgroundColor = XLColor.FromHtml("#007bff");
                baslik.SetAutoFilter();

                // 3. Kenarlıklar
                tabloAraligi.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                tabloAraligi.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // 4. Zebra Deseni (Uçuk Mavi - #E3F2FD)
                for (int i = 2; i < row; i++)
                {
                    if (i % 2 == 0)
                    {
                        worksheet.Row(i).Cells(1, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#E3F2FD");
                    }
                }

                // 5. Genişlikleri Otomatik Ayarla
                worksheet.Columns().AdjustToContents();

                // --- 6. NEFES PAYI (FİLTRE OKLARI İÇİN) ---
                // Burada 6 sütun olduğu için döngü 6'ya kadar dönüyor
                for (int i = 1; i <= 6; i++)
                {
                    worksheet.Column(i).Width += 3;
                }

                // --- D. İNDİRME ---
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"MerkezDepo_{DateTime.Now:dd-MM-yyyy}.xlsx");
                }
            }
        }
    }
}


