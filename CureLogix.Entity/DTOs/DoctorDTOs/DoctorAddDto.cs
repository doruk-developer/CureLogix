namespace CureLogix.Entity.DTOs.DoctorDTOs
{
    public class DoctorAddDto
    {
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Specialty { get; set; }
        public int RoleType { get; set; }
        public int? HospitalId { get; set; } // Konsey üyeleri hastanesiz olabilir
        public string Email { get; set; }
    }
}
