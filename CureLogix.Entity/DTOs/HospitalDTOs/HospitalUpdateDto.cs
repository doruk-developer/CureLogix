namespace CureLogix.Entity.DTOs.HospitalDTOs
{
    public class HospitalUpdateDto
    {
        public int Id { get; set; } // ID boş olamaz, bu kalıyor.

        public string? Name { get; set; } // ?
        public string? City { get; set; } // ?

        public int? MainStorageCapacity { get; set; } // ?
        public int? WasteStorageCapacity { get; set; } // ?

        public decimal? OccupancyRate { get; set; } // Zaten ? vardı, kalsın.
        public bool? IsActive { get; set; } // ?
    }
}