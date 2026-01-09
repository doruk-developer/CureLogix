using CureLogix.Entity.DTOs.WarehouseDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CureLogix.WebUI.Models
{
    public class CentralStockViewModel
    {
        public CentralStockAddDto Data { get; set; } = new CentralStockAddDto();
        public List<SelectListItem> MedicineList { get; set; }
    }
}