namespace CureLogix.Entity.DTOs.HospitalDTOs
{
    public class HospitalUpdateDto
    {
        public int Id { get; set; } // GÜNCELLEME İÇİN BU ŞART!
        public string Name { get; set; }
        public string City { get; set; }
        public int MainStorageCapacity { get; set; }
        public int WasteStorageCapacity { get; set; }
        public decimal? OccupancyRate { get; set; }
        public bool IsActive { get; set; }
    }
}