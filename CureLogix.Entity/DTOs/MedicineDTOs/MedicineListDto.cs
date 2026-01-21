namespace CureLogix.Entity.DTOs.MedicineDTOs
{
    public class MedicineListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? Unit { get; set; }
        public int? CriticalStockLevel { get; set; }
        public bool? RequiresColdChain { get; set; }
    }
}