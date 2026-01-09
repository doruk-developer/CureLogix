namespace CureLogix.Entity.DTOs.DoctorDTOs
{
    public class DoctorListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }     // Ünvan (Prof. Dr.)
        public string Specialty { get; set; } // Branş
        public int RoleType { get; set; }     // Enum olarak kullanacağız
        public string HospitalName { get; set; } // İlişkili tablodan gelecek
    }
}