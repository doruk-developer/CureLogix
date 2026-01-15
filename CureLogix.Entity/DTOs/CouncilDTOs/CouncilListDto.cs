namespace CureLogix.Entity.DTOs.CouncilDTOs
{
    public class CouncilListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string DoctorName { get; set; } // ID yerine İsim
    }
}