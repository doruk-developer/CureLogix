namespace CureLogix.Entity.DTOs.HospitalDTOs
{
    public class HospitalListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int MainStorageCapacity { get; set; }
        public decimal? OccupancyRate { get; set; }
        // Bak, 'WasteStorageCapacity' veya 'IsActive' alanlarını almadık.
        // Çünkü listede göstermek istemiyoruz. DTO'nun olayı bu!
    }
}