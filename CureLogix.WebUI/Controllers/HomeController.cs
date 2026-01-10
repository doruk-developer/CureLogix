using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Entity.DTOs.WarehouseDTOs;
using CureLogix.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly IDoctorService _doctorService;
        private readonly IMedicineService _medicineService;
        private readonly ITreatmentProtocolService _protocolService;
        private readonly ISupplyRequestService _supplyService;
        private readonly ICentralWarehouseService _warehouseService;
        private readonly IMapper _mapper;

        public HomeController(
            IHospitalService hospitalService,
            IDoctorService doctorService,
            IMedicineService medicineService,
            ITreatmentProtocolService protocolService,
            ISupplyRequestService supplyService,
            ICentralWarehouseService warehouseService,
            IMapper mapper)
        {
            _hospitalService = hospitalService;
            _doctorService = doctorService;
            _medicineService = medicineService;
            _protocolService = protocolService;
            _supplyService = supplyService;
            _warehouseService = warehouseService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var model = new AdvancedDashboardViewModel();

            // 1. KPI Verileri
            model.TotalHospitals = _hospitalService.TGetList().Count;
            model.TotalDoctors = _doctorService.TGetList().Count;

            var requests = _supplyService.TGetList();
            model.PendingRequests = requests.Count(x => x.Status == 0); // Bekleyenler

            var stocks = _warehouseService.TGetList();
            // Kritik seviye: 1000 adetin altý (Simülasyon kuralý)
            model.CriticalStockCount = stocks.Count(x => x.Quantity < 1000);

            // 2. Grafikler
            // A) Hastane Doluluk
            var hospitals = _hospitalService.TGetList();
            model.HospitalNames = hospitals.Select(x => x.Name).ToList();
            model.OccupancyRates = hospitals.Select(x => x.OccupancyRate ?? 0).ToList();

            // B) Talep Durumlarý
            model.WaitingReq = model.PendingRequests;
            model.ApprovedReq = requests.Count(x => x.Status == 1);
            model.RejectedReq = 5; // Simülasyon (DB'de red sütunu yoksa)

            // C) Radar Chart (Kategori Analizi - Simülasyon)
            model.MedicineCategories = new List<string> { "Antibiyotik", "Antiviral", "Aþý", "Analjezik", "Solunum", "Kardiyak" };
            model.CategoryStockLevels = new List<int> { 85, 40, 90, 65, 30, 75 }; // Rastgele simülasyon verileri

            // 3. Tablolar
            // Kritik Stoklar (Ýlk 5)
            var criticals = stocks.Where(x => x.Quantity < 5000).OrderBy(x => x.Quantity).Take(5).ToList();
            model.CriticalMedicines = _mapper.Map<List<CentralStockListDto>>(criticals);

            // Son Aktiviteler (Simülasyon - Normalde Audit tablosundan gelir)
            model.RecentActivities = new List<string>
            {
                "Dr. Kemal Sayar sisteme giriþ yaptý.",
                "Merkez Depo'ya 10.000 adet ViruGuard giriþi yapýldý.",
                "Acil Durum: Ankara Þehir Hastanesi oksijen tüpü talep etti.",
                "Konsey, RS-24 protokolünü onayladý.",
                "Sistem yedeklemesi baþarýyla tamamlandý."
            };

            return View(model);
        }
    }
}