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

        // 1. Hastane Güncelleme Sayfasını Getir (GET)
        [HttpGet]
        public IActionResult Update(int id)
        {
            var value = _hospitalService.TGetById(id);
            if (value == null) return RedirectToAction("Index");

            // Entity'den UpdateDto'ya çevir
            var updateDto = _mapper.Map<HospitalUpdateDto>(value);
            return View(updateDto);
        }

        // 2. Hastane Güncelleme İşlemini Kaydet (POST)
        [HttpPost]
        public IActionResult Update(HospitalUpdateDto p)
        {
            HospitalUpdateValidator validator = new HospitalUpdateValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                var hospital = _mapper.Map<Hospital>(p);
                _hospitalService.TUpdate(hospital);
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
            }
            return View(p);
        }

        // Hastane Silme İşlemi
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // 1. Silinecek kaydı bul
            var value = _hospitalService.TGetById(id);

            // 2. Kayıt varsa sil
            if (value != null)
            {
                // (İleride buraya "Bu hastanede doktor var mı?" kontrolü eklenebilir)
                _hospitalService.TDelete(value);
            }

            // 3. Listeye geri dön
            return RedirectToAction("Index");
        }
    }
}