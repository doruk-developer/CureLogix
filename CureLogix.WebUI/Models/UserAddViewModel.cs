using CureLogix.Entity.DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem için gerekli

namespace CureLogix.WebUI.Models
{
    public class UserAddViewModel
    {
        // Form verilerini taşıyan DTO
        public UserAddDto Data { get; set; } = new UserAddDto();

        // Dropdown için Ünvan Listesi
        public List<SelectListItem>? TitleList { get; set; }
    }
}