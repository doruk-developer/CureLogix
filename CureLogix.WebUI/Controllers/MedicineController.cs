using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.MedicineDTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class MedicineController : Controller
    {
        private readonly IMedicineService _medicineService;
        private readonly IMapper _mapper;

        public MedicineController(IMedicineService medicineService, IMapper mapper)
        {
            _medicineService = medicineService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var values = _medicineService.TGetList();
            var medicineList = _mapper.Map<List<MedicineListDto>>(values);
            return View(medicineList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(MedicineAddDto p)
        {
            MedicineValidator validator = new MedicineValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                var medicine = _mapper.Map<Medicine>(p);
                _medicineService.TAdd(medicine);
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

    }
}