namespace CureLogix.Entity.DTOs.CouncilDTOs
{
    public class ProtocolDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DiseaseName { get; set; } // ID değil İsim lazım
        public string DoctorName { get; set; }  // ID değil İsim lazım
        public DateTime CreatedDate { get; set; }

        // Reçetedeki ilaçları göstermek için
        // Daha önce oluşturduğumuz ProtocolMedicineDto'yu kullanabiliriz
        // veya sadece okuma amaçlı yeni bir DTO yapabiliriz. Şimdilik basitleştirelim:
        public List<ProtocolMedicineViewDto> Medicines { get; set; }
    }

    public class ProtocolMedicineViewDto
    {
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
    }
}