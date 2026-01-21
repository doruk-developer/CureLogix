namespace CureLogix.Entity.DTOs.MedicineDTOs
{
    public class MedicineUpdateDto
    {
        public int Id { get; set; } // ID zorunludur, bu kalıyor.

        public string? Name { get; set; }

        public string? ActiveIngredient { get; set; }

        public string? Unit { get; set; }

        public int? ShelfLifeDays { get; set; }

        public int? CriticalStockLevel { get; set; }

        public bool? RequiresColdChain { get; set; }
    }
}