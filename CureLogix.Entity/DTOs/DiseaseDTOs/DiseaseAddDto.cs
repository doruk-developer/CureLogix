namespace CureLogix.Entity.DTOs.DiseaseDTOs
{
    public class DiseaseAddDto
    {
        // "Name" boş gelirse .NET değil, bizim Validator hata versin diye '?' koyduk.
        public string? Name { get; set; }

        public string? Code { get; set; } // ICD-10

        public string? Description { get; set; }

        // DİKKAT: Dropdown'dan sayısal değer (Enum ID) geleceği için int? yaptık.
        // String kalırsa model binder hata verebilir.
        public int? RiskLevel { get; set; }
    }
}