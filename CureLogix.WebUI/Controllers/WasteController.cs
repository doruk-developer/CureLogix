using AutoMapper;
using ClosedXML.Excel;
using CureLogix.Business.Abstract;
using CureLogix.Entity.DTOs.WasteDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CureLogix.WebUI.Controllers
{
    public class WasteController : Controller
    {
        private readonly IWasteReportService _wasteService;
        private readonly IMapper _mapper;

        public WasteController(IWasteReportService wasteService, IMapper mapper)
        {
            _wasteService = wasteService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var values = _wasteService.TGetList();
            var list = _mapper.Map<List<WasteReportListDto>>(values);
            return View(list);
        }

        // AKILLI TARAMA: Miadı dolanları bulur ve atığa ayırır
        public IActionResult ScanAndMove()
        {
            _wasteService.MoveExpiredToWaste();
            TempData["Success"] = "Sistem tarandı. Miadı dolan tüm ilaçlar 'Tıbbi Atık' deposuna transfer edildi.";
            return RedirectToAction("Index");
        }

        public IActionResult ExportExcel()
        {
            // 1. Ham veriyi çek (Entity)
            var values = _wasteService.TGetList();

            // 2. DTO'ya Dönüştür (Kritik Adım: İsimler burada oluşuyor)
            var reports = _mapper.Map<List<WasteReportListDto>>(values);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("İmha Tutanakları");

                // --- A. BAŞLIKLAR ---
                worksheet.Cell(1, 1).Value = "Tarih";
                worksheet.Cell(1, 2).Value = "Hastane";
                worksheet.Cell(1, 3).Value = "İmha Edilen Ürün";
                worksheet.Cell(1, 4).Value = "Miktar";
                worksheet.Cell(1, 5).Value = "Gerekçe";

                // --- B. VERİLERİ DÖK ---
                int row = 2;
                foreach (var item in reports)
                {
                    worksheet.Cell(row, 1).Value = item.DisposalDate;
                    worksheet.Cell(row, 2).Value = item.HospitalName; // Null ise boş kalır
                    worksheet.Cell(row, 3).Value = item.MedicineName;

                    // Miktar (Sayısal)
                    worksheet.Cell(row, 4).Value = item.Quantity;
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";

                    worksheet.Cell(row, 5).Value = item.Reason;

                    row++;
                }

                // --- C. PROFESYONEL STİL ---
                var tabloAraligi = worksheet.Range(1, 1, row - 1, 5);

                // 1. Hizalama (Tam Ortala)
                tabloAraligi.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                tabloAraligi.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // 2. Başlık Stili
                var baslik = worksheet.Range("A1:E1");
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
                        worksheet.Row(i).Cells(1, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#E3F2FD");
                    }
                }

                // 5. Sütun Genişliklerini Otomatik Ayarla
                worksheet.Columns().AdjustToContents();

                // --- 6. FİLTRE BUTONU İÇİN NEFES PAYI (YENİ) ---
                for (int i = 1; i <= 5; i++)
                {
                    worksheet.Column(i).Width += 3;
                }

                // --- D. İNDİRME ---
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ImhaTutanaklari_{DateTime.Now:dd-MM-yyyy}.xlsx");
                }
            }
        }
    }
}