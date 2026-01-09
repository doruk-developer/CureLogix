using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.WebUI.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CureLogix.WebUI.Controllers
{
    public class ProtocolController : Controller
    {
        private readonly ITreatmentProtocolService _protocolService;
        private readonly IDiseaseService _diseaseService;
        private readonly IDoctorService _doctorService;
        private readonly IMedicineService _medicineService;
        private readonly IMapper _mapper;

        public ProtocolController(
            ITreatmentProtocolService protocolService,
            IDiseaseService diseaseService,
            IDoctorService doctorService,
            IMedicineService medicineService,
            IMapper mapper)
        {
            _protocolService = protocolService;
            _diseaseService = diseaseService;
            _doctorService = doctorService;
            _medicineService = medicineService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            // Şimdilik boş liste dönüyoruz (Create kısmına odaklandık)
            return View(new List<TreatmentProtocol>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            // 1. Boş bir ViewModel oluşturuyoruz
            var viewModel = new ProtocolCreateViewModel();

            // 2. Dropdown verilerini servisten çekip ViewModel'e dolduruyoruz
            viewModel.DiseaseList = new SelectList(_diseaseService.TGetList(), "Id", "Name");
            viewModel.DoctorList = new SelectList(_doctorService.TGetList(), "Id", "FullName");

            // 3. İlaç listesini (JavaScript için) ViewModel'e ekliyoruz
            viewModel.MedicineSource = _medicineService.TGetList();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(ProtocolCreateViewModel model)
        {
            // Validasyon işlemini DTO üzerinden yapıyoruz (model.FormData)
            ProtocolValidator validator = new ProtocolValidator();
            ValidationResult results = validator.Validate(model.FormData);

            if (results.IsValid)
            {
                // DTO -> Entity dönüşümü
                var protocol = _mapper.Map<TreatmentProtocol>(model.FormData);

                // Varsayılan değerler
                protocol.Status = 0; // Onay Bekliyor
                protocol.CreatedDate = DateTime.Now;

                // Veritabanına Kayıt (Master + Detail aynı anda)
                _protocolService.TAdd(protocol);

                return RedirectToAction("Index", "Home"); // Başarılıysa Dashboard'a dön
            }
            else
            {
                // Hataları ekrana bas
                foreach (var item in results.Errors)
                {
                    // Hata mesajını doğru inputun altına yazdırmak için PropertyName'i düzeltiyoruz
                    // Örn: "Title" hatasını "FormData.Title" alanına eşliyoruz
                    ModelState.AddModelError($"FormData.{item.PropertyName}", item.ErrorMessage);
                }
            }

            // HATA VARSA: Dropdownları TEKRAR doldurmak zorundayız (Yoksa boş gelir)
            model.DiseaseList = new SelectList(_diseaseService.TGetList(), "Id", "Name");
            model.DoctorList = new SelectList(_doctorService.TGetList(), "Id", "FullName");
            model.MedicineSource = _medicineService.TGetList();

            return View(model);
        }
    }
}