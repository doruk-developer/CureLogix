namespace CureLogix.Entity.DTOs.WarehouseDTOs
{
    public class CentralStockAddDto
    {
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public string BatchNo { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}