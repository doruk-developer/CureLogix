using CureLogix.Business.Abstract;  // Servis Interface'leri burada (IHospitalService vb.)
using CureLogix.WebUI.Models;       // ViewModel burada
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly IDoctorService _doctorService;
        private readonly IMedicineService _medicineService;
        private readonly ITreatmentProtocolService _protocolService; // Yeni
        private readonly IDiseaseService _diseaseService; // Yeni

        public HomeController(
            IHospitalService hospitalService,
            IDoctorService doctorService,
            IMedicineService medicineService,
            ITreatmentProtocolService protocolService,
            IDiseaseService diseaseService)
        {
            _hospitalService = hospitalService;
            _doctorService = doctorService;
            _medicineService = medicineService;
            _protocolService = protocolService;
            _diseaseService = diseaseService;
        }

        public IActionResult Index()
        {
            // Tüm verileri çek
            var hospitals = _hospitalService.TGetList();
            var doctors = _doctorService.TGetList();
            var protocols = _protocolService.TGetList();

            var model = new DashboardViewModel
            {
                // 1. Kart Verileri
                TotalHospitals = hospitals.Count,
                TotalDoctors = doctors.Count,
                TotalMedicines = _medicineService.TGetList().Count,
                TotalProtocols = protocols.Count,

                // 2. Grafik: Hastane Doluluk Oranlarý
                HospitalNames = hospitals.Select(x => x.Name).ToList(),
                OccupancyRates = hospitals.Select(x => x.OccupancyRate ?? 0).ToList(), // Null ise 0 al

                // 3. Grafik: Protokol Durumlarý (0:Bekleyen, 1:Onay, 2:Red)
                PendingProtocols = protocols.Count(x => x.Status == 0),
                ApprovedProtocols = protocols.Count(x => x.Status == 1),
                RejectedProtocols = protocols.Count(x => x.Status == 2),

                // 4. Grafik: Doktor Branþ Daðýlýmý (Gruplama)
                DoctorSpecialties = doctors.GroupBy(d => d.Specialty)
                                           .Select(g => g.Key).ToList(),
                DoctorSpecialtyCounts = doctors.GroupBy(d => d.Specialty)
                                               .Select(g => g.Count()).ToList()
            };

            return View(model);
        }
    }
}