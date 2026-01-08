namespace CureLogix.Entity.DTOs.DiseaseDTOs
{
    public class DiseaseAddDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string RiskLevel { get; set; } // Bunu Dropdown'dan seçtireceğiz
    }
}