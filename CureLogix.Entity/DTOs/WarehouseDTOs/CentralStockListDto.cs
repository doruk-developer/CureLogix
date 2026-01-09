namespace CureLogix.Entity.DTOs.WarehouseDTOs
{
    public class CentralStockListDto
    {
        public int Id { get; set; }
        public string MedicineName { get; set; } // İlaç adı
        public string MedicineUnit { get; set; } // Birim (Kutu)
        public int Quantity { get; set; }
        public string BatchNo { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}