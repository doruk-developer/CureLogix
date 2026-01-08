using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Entity.DTOs.HospitalDTOs;
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
    }
}