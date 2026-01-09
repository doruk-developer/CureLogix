namespace CureLogix.Entity.DTOs.ProtocolDTOs
{
    // ANA FORM (BAŞLIK)
    public class ProtocolCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DiseaseId { get; set; }
        public int DoctorId { get; set; } // Şimdilik dropdown'dan seçtireceğiz

        // Burası kritik: Protokolün içindeki ilaçlar listesi
        public List<ProtocolMedicineDto> Medicines { get; set; } = new List<ProtocolMedicineDto>();
    }

    // DETAY (İLAÇ SATIRLARI)
    public class ProtocolMedicineDto
    {
        public int MedicineId { get; set; }
        public int RequiredQuantity { get; set; }
        public string DosageInstructions { get; set; } // Örn: "Günde 2x1 Tok"
    }
}