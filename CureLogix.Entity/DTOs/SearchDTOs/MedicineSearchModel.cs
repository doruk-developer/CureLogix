namespace CureLogix.Entity.DTOs.SearchDTOs
{
    public class MedicineSearchModel
    {
        // Standart büyük 'Id'ye geri döndük
        public int Id { get; set; }

        public string Name { get; set; }
        public string ActiveIngredient { get; set; }
        public string Unit { get; set; }
        public int StockQuantity { get; set; }
        public bool IsCritical { get; set; }
        public bool RequiresColdChain { get; set; }
    }
}