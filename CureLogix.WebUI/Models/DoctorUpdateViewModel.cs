using CureLogix.Entity.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CureLogix.WebUI.Models
{
    public class DoctorUpdateViewModel
    {
        public DoctorUpdateDto Data { get; set; } = new DoctorUpdateDto();

        // Dropdown Listeleri
        public List<SelectListItem> HospitalList { get; set; }
        public List<SelectListItem> RoleList { get; set; }
    }
}