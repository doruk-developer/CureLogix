namespace CureLogix.Entity.DTOs.MedicineDTOs
{
    public class MedicineListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ActiveIngredient { get; set; } // Etken Madde
        public string Unit { get; set; }             // Kutu/Şişe
        public int? CriticalStockLevel { get; set; } // Kritik seviye uyarısı için lazım
        public bool? RequiresColdChain { get; set; } // Kar tanesi ikonu için lazım ❄️
    }
}