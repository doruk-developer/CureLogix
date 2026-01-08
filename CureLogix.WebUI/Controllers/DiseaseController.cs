using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Business.ValidationRules;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.DiseaseDTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class DiseaseController : Controller
    {
        private readonly IDiseaseService _diseaseService;
        private readonly IMapper _mapper;

        public DiseaseController(IDiseaseService diseaseService, IMapper mapper)
        {
            _diseaseService = diseaseService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var values = _diseaseService.TGetList();
            var list = _mapper.Map<List<DiseaseListDto>>(values);
            return View(list);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(DiseaseAddDto p)
        {
            DiseaseValidator validator = new DiseaseValidator();
            ValidationResult results = validator.Validate(p);

            if (results.IsValid)
            {
                var disease = _mapper.Map<Disease>(p);
                _diseaseService.TAdd(disease);
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