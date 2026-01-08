using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules; // Validator sınıfımız için
using CureLogix.Entity.Concrete;          // Hospital entity'si için
using CureLogix.Entity.DTOs.HospitalDTOs;
using FluentValidation.Results; // ValidationResult için gerekli
using Microsoft.AspNetCore.Mvc;


namespace CureLogix.WebUI.Controllers
{
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly IMapper _mapper; // AutoMapper Servisi

        public HospitalController(IHospitalService hospitalService, IMapper mapper)
        {
            _hospitalService = hospitalService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            // 1. Veritabanından HAM veriyi çek (Entity Listesi)
            var values = _hospitalService.TGetList();

            // 2. AutoMapper ile DTO Listesine çevir (Tek satır!)
            var hospitalList = _mapper.Map<List<HospitalListDto>>(values);

            // 3. Ekrana DTO gönder
            return View(hospitalList);
        }

        // ... Constructor ve Index Metodu aynen kalıyor ...

        // 1. SAYFAYI GETİRME (GET)
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // 2. VERİYİ KAYDETME (POST)
        [HttpPost]
        public IActionResult Add(HospitalAddDto p)
        {
            // Validasyon Kontrolü
            HospitalValidator validator = new HospitalValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                // 1. Validasyondan geçtiyse DTO'yu Entity'e çevir
                var hospitalEntity = _mapper.Map<Hospital>(p);

                // 2. Varsayılan değerleri ata (örn: Doluluk oranı başta 0'dır)
                hospitalEntity.OccupancyRate = 0;
                hospitalEntity.IsActive = true;

                // 3. Veritabanına kaydet
                _hospitalService.TAdd(hospitalEntity);

                // 4. Listeye geri dön
                return RedirectToAction("Index");
            }
            else
            {
                // Hata varsa hataları ekrana bas
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
            }

            // Hata varsa sayfayı tekrar göster (kullanıcı girdiği verileri kaybetmesin)
            return View(p);
        }
    }
}