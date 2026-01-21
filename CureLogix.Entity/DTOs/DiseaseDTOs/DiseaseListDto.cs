namespace CureLogix.Entity.DTOs.DiseaseDTOs
{
    public class DiseaseListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        // Listelerken Enum'ın adını (Örn: "Pandemik") yazdırıyorsan string kalabilir.
        // Ama null gelirse patlamasın diye ? ekledik.
        public string? RiskLevel { get; set; }
    }
}