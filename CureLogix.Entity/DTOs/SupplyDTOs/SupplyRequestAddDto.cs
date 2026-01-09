namespace CureLogix.Entity.DTOs.SupplyDTOs
{
    public class SupplyRequestAddDto
    {
        public int HospitalId { get; set; }
        public int MedicineId { get; set; }
        public int RequestQuantity { get; set; }
        // Başhekim ID'si şimdilik otomatik atanacak
    }
}