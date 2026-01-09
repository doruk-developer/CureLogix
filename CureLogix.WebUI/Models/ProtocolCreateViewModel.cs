using CureLogix.Entity.Concrete; // Medicine listesi için
using CureLogix.Entity.DTOs.ProtocolDTOs;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList için

namespace CureLogix.WebUI.Models
{
    public class ProtocolCreateViewModel
    {
        // 1. Kullanıcının dolduracağı asıl veri (DTO)
        public ProtocolCreateDto FormData { get; set; } = new ProtocolCreateDto();

        // 2. Dropdown için gerekli listeler (ViewBag yerine buradalar)
        public SelectList DiseaseList { get; set; }
        public SelectList DoctorList { get; set; }

        // 3. JavaScript'e göndereceğimiz İlaç Listesi
        public List<Medicine> MedicineSource { get; set; }
    }
}