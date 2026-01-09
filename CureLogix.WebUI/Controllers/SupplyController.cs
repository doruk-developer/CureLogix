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

        public SupplyController(ISupplyRequestService supplyService, ICentralWarehouseService warehouseService, IHospitalService hospitalService, IMedicineService medicineService, IMapper mapper)
        {
            _supplyService = supplyService;
            _warehouseService = warehouseService;
            _hospitalService = hospitalService;
            _medicineService = medicineService;
            _mapper = mapper;
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
                request.RequestingDoctorId = 1; // Şimdilik 1 nolu doktor (Ahmet Mehmet) istiyor varsayalım

                _supplyService.TAdd(request);
                return RedirectToAction("Index");
            }
            // Hata varsa ViewBag'leri doldur
            ViewBag.Hospitals = new SelectList(_hospitalService.TGetList(), "Id", "Name");
            ViewBag.Medicines = new SelectList(_medicineService.TGetList(), "Id", "Name");
            return View(p);
        }

        // 4. KRİTİK NOKTA: ONAYLA VE FEFO'YU ÇALIŞTIR
        public IActionResult Approve(int id)
        {
            var request = _supplyService.TGetById(id);

            try
            {
                // FEFO Algoritmasını Çalıştır (Stoktan Düş)
                _warehouseService.DistributeStockByFEFO(request.MedicineId, request.RequestQuantity);

                // Talep Durumunu Güncelle
                request.Status = 1; // Onaylandı
                request.ApprovedQuantity = request.RequestQuantity;
                _supplyService.TUpdate(request);

                TempData["Success"] = "Talep onaylandı ve stoktan FEFO kuralına göre düşüldü.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}