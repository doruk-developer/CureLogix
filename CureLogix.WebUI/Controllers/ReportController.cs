using ClosedXML.Excel;          // Sadece Excel kütüphanesi kaldı
using CureLogix.Business.Abstract;
using CureLogix.Entity.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CureLogix.WebUI.Controllers
{
    public class ReportController : Controller
    {
        private readonly ISupplyRequestService _supplyService;
        private readonly IMedicineService _medicineService;
        private readonly IHospitalService _hospitalService;

        public ReportController(ISupplyRequestService supplyService, IMedicineService medicineService, IHospitalService hospitalService)
        {
            _supplyService = supplyService;
            _medicineService = medicineService;
            _hospitalService = hospitalService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // =============================================================
        // 📊 AKILLI EXCEL MOTORU (Kusursuz Çalışan Versiyon)
        // =============================================================
        public IActionResult ExportExcel_Consumption()
        {
            // 1. VERİYİ HAZIRLA (Onaylı Talepler)
            var reportData = _supplyService.TGetList()
                .Where(x => x.Status == 1) // Sadece Onaylananlar
                .OrderByDescending(x => x.RequestDate)
                .ToList();

            // DataTable oluştur
            var dataTable = new DataTable("Haftalık Tüketim");
            dataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("İşlem Tarihi"),
                new DataColumn("Talep Eden Hastane"),
                new DataColumn("Transfer Edilen İlaç"),
                new DataColumn("Adet / Miktar"),
                new DataColumn("İşlemi Yapan")
            });

            // Verileri doldur
            foreach (var item in reportData)
            {
                var hospital = _hospitalService.TGetById(item.HospitalId);
                var medicine = _medicineService.TGetById(item.MedicineId);

                dataTable.Rows.Add(
                    item.RequestDate?.ToString("dd.MM.yyyy HH:mm"),
                    hospital?.Name,
                    medicine?.Name,
                    item.ApprovedQuantity,
                    "Dr. Yönetici"
                );
            }

            // 2. EXCEL DOSYASINI OLUŞTUR
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dataTable);

                // --- STİL VE AYARLAR ---

                // Tablo temasını sıfırla ve filtreyi aç
                var table = ws.Tables.FirstOrDefault();
                if (table != null)
                {
                    table.Theme = XLTableTheme.None;
                    table.ShowAutoFilter = true;
                }

                // A) Sütun Genişliklerini Otomatik Ayarla
                ws.Columns().AdjustToContents();

                // B) BAŞLIK SATIRI TASARIMI
                var headerRange = ws.Range(1, 1, 1, dataTable.Columns.Count);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#17a2b8"); // CureLogix Mavisi
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // C) ÇERÇEVELER VE ZEBRA DESENİ
                var dataRange = ws.RangeUsed();

                if (dataRange != null) // Null kontrolü
                {
                    // İç Çizgiler (İnce)
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorderColor = XLColor.Gray;

                    // Dış Çerçeve (Kalın)
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    dataRange.Style.Border.OutsideBorderColor = XLColor.Black;

                    // Zebra Deseni
                    for (int i = 2; i <= dataRange.RowCount(); i++)
                    {
                        if (i % 2 == 0)
                            ws.Row(i).Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f9fa"); // Gri
                        else
                            ws.Row(i).Style.Fill.BackgroundColor = XLColor.White; // Beyaz
                    }
                }

                // D) Sayfa Yapısı
                ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                ws.PageSetup.FitToPages(1, 0);

                // 3. DOSYAYI İNDİR
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TuketimRaporu_{DateTime.Now:ddMMyyyy}.xlsx");
                }
            }
        }
    }
}