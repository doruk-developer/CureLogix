using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.SupplyDTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CureLogix.WebUI.Controllers
{
    public class SupplyController : Controller
    {
        private readonly ISupplyRequestService _supplyService;
        private readonly ICentralWarehouseService _warehouseService;
        private readonly IHospitalService _hospitalService;
        private readonly IMedicineService _medicineService;
        private readonly IMapper _mapper;
        private readonly IVehicleService _vehicleService; // Yeni eklenen servis

        // TEK VE TAM CONSTRUCTOR
        public SupplyController(
            ISupplyRequestService supplyService,
            ICentralWarehouseService warehouseService,
            IHospitalService hospitalService,
            IMedicineService medicineService,
            IMapper mapper,
            IVehicleService vehicleService) // Parametre eklendi
        {
            _supplyService = supplyService;
            _warehouseService = warehouseService;
            _hospitalService = hospitalService;
            _medicineService = medicineService;
            _mapper = mapper;
            _vehicleService = vehicleService; // Atama yapıldı
        }

        // 1. Talepleri Listele
        public IActionResult Index()
        {
            // Bekleyen (Status=0) talepleri getir
            var list = _supplyService.TGetList().Where(x => x.Status == 0).ToList();
            return View(list);
        }

        // 2. Talep Oluşturma Sayfası (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Hospitals = new SelectList(_hospitalService.TGetList(), "Id", "Name");
            ViewBag.Medicines = new SelectList(_medicineService.TGetList(), "Id", "Name");
            return View();
        }

        // 3. Talep Kaydet (POST)
        [HttpPost]
        public IActionResult Create(SupplyRequestAddDto p)
        {
            SupplyRequestValidator validator = new SupplyRequestValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                var request = _mapper.Map<SupplyRequest>(p);
                request.Status = 0; // Bekliyor
                request.RequestDate = DateTime.Now;
                request.RequestingDoctorId = 1; // Simülasyon ID

                _supplyService.TAdd(request);
                return RedirectToAction("Index");
            }

            ViewBag.Hospitals = new SelectList(_hospitalService.TGetList(), "Id", "Name");
            ViewBag.Medicines = new SelectList(_medicineService.TGetList(), "Id", "Name");
            return View(p);
        }

        // 4. KRİTİK METOT: ONAYLA, ARAÇ KONTROLÜ VE FEFO
        [HttpPost]
        public IActionResult Approve(int id, int vehicleId)
        {
            var request = _supplyService.TGetById(id);
            var medicine = _medicineService.TGetById(request.MedicineId);
            var vehicle = _vehicleService.TGetById(vehicleId);

            try
            {
                // A) SOĞUK ZİNCİR KONTROLÜ (IOT SİMÜLASYONU)
                if (medicine.RequiresColdChain == true && vehicle.HasCoolingSystem == false)
                {
                    TempData["Error"] = $"⛔ GÜVENLİK İHLALİ: '{medicine.Name}' ilacı soğuk zincir gerektirir! '{vehicle.PlateNumber}' plakalı araçta soğutucu sistem yok.";
                    return RedirectToAction("Index");
                }

                // B) FEFO ALGORİTMASI (STOK DÜŞÜMÜ)
                _warehouseService.DistributeStockByFEFO(request.MedicineId, request.RequestQuantity);

                // C) DURUM GÜNCELLEME VE ARAÇ ATAMA
                request.Status = 1; // Onaylandı
                request.ApprovedQuantity = request.RequestQuantity;
                request.AssignedVehicleId = vehicleId;

                _supplyService.TUpdate(request);

                TempData["Success"] = $"✅ İşlem Başarılı! İlaçlar '{vehicle.PlateNumber}' plakalı araca yüklendi ve yola çıktı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}