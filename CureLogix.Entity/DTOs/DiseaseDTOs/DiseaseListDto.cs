namespace CureLogix.Entity.DTOs.DiseaseDTOs
{
    public class DiseaseListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }      // Örn: U07.1
        public string RiskLevel { get; set; } // Örn: Pandemik
    }
}