namespace CureLogix.Entity.DTOs.DiseaseDTOs
{
    public class DiseaseUpdateDto
    {
        public int Id { get; set; } // Güncelleme için şart
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string RiskLevel { get; set; }
    }
}