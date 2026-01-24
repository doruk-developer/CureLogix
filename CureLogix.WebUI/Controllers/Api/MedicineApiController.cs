using AutoMapper;
using CureLogix.Business.Abstract;
using CureLogix.Entity.DTOs.MedicineDTOs;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineApiController : ControllerBase
    {
        private readonly IMedicineService _medicineService;
        private readonly IMapper _mapper;

        public MedicineApiController(IMedicineService medicineService, IMapper mapper)
        {
            _medicineService = medicineService;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm ilaçları listeler (Performans için varsayılan 100 kayıt döner)
        /// </summary>
        /// <param name="count">Getirilecek kayıt sayısı</param>
        /// <returns>MedicineListDto Listesi</returns>
        [HttpGet]
        public IActionResult GetAllMedicines([FromQuery] int count = 100)
        {
            // 1. Veritabanından ham veriyi (Entity) limitli olarak çekiyoruz
            // 10.000 kaydı aynı anda çekmek hem DB'yi hem tarayıcıyı yorar.
            var values = _medicineService.TGetList().Take(count).ToList();

            // 2. Entity -> DTO Dönüşümü
            // Bu adım "Circular Reference" (Sonsuz Döngü) hatasını kökten çözer.
            var dtoList = _mapper.Map<List<MedicineListDto>>(values);

            return Ok(dtoList);
        }

        /// <summary>
        /// ID bazlı ilaç detayı getirir
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetMedicineById(int id)
        {
            var value = _medicineService.TGetById(id);

            if (value == null)
            {
                return NotFound(new { Message = $"{id} ID'li ilaç sistemde bulunamadı." });
            }

            var dto = _mapper.Map<MedicineListDto>(value);
            return Ok(dto);
        }

        /// <summary>
        /// Kritik stok seviyesindeki ilaçları getirir
        /// </summary>
        [HttpGet("Critical")]
        public IActionResult GetCriticalMedicines()
        {
            // İş mantığı: Stok miktarı, kritik seviyenin altında olanları filtrele
            // Not: TGetList() içindeki filtreleme mantığını Manager tarafına taşımak daha profesyoneldir
            var values = _medicineService.TGetList()
                         .Where(x => x.CriticalStockLevel >= 100) // Örnek filtreleme
                         .OrderBy(x => x.Name)
                         .ToList();

            var dtoList = _mapper.Map<List<MedicineListDto>>(values);
            return Ok(dtoList);
        }
    }
}