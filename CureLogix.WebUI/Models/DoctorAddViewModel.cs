using CureLogix.Entity.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem için gerekli

namespace CureLogix.WebUI.Models
{
    public class DoctorAddViewModel
    {
        // 1. Kullanıcının dolduracağı form verileri (DTO)
        public DoctorAddDto Data { get; set; } = new DoctorAddDto();

        // 2. Dropdown Listeleri (ViewBag yerine bunlar kullanılacak)
        public List<SelectListItem> HospitalList { get; set; }
        public List<SelectListItem> RoleList { get; set; }
    }
}