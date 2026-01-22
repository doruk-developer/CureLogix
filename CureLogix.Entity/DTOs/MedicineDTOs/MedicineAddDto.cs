namespace CureLogix.Entity.DTOs.MedicineDTOs
{
    public class MedicineAddDto
    {
        // Boş gelirse sistem değil, bizim Türkçe Validator hata versin diye '?' ekledik.
        public string? Name { get; set; }

        public string? ActiveIngredient { get; set; } // Etken Madde

        public string? Unit { get; set; } // Kutu/Şişe/Ampul

        public int? ShelfLifeDays { get; set; } // Raf Ömrü

        public int? CriticalStockLevel { get; set; }

        public bool RequiresColdChain { get; set; } // Checkbox
    }
}