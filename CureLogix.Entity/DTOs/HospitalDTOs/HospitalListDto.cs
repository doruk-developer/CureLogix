namespace CureLogix.Entity.DTOs.HospitalDTOs
{
    public class HospitalListDto
    {
        public int Id { get; set; }

        // Listeleme yaparken veritabanından null gelirse patlamasın diye önlem:
        public string? City { get; set; }
        public string? Name { get; set; }

        public int? MainStorageCapacity { get; set; }
        public decimal? OccupancyRate { get; set; }
    }
}