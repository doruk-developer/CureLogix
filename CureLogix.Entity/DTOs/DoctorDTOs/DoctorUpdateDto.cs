namespace CureLogix.Entity.DTOs.DoctorDTOs
{
    public class DoctorUpdateDto
    {
        public int Id { get; set; }

        public string? FullName { get; set; }
        public string? Title { get; set; }
        public string? Specialty { get; set; }

        public int? RoleType { get; set; }
        public int? HospitalId { get; set; }

        public string? Email { get; set; }
    }
}