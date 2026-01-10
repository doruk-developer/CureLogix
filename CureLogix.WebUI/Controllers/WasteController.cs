using AutoMapper;
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
    }
}