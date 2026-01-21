namespace CureLogix.Entity.DTOs.DiseaseDTOs
{
    public class DiseaseUpdateDto
    {
        public int Id { get; set; } // ID zorunludur.

        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? RiskLevel { get; set; }
    }
}