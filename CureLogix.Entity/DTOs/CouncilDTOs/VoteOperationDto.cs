namespace CureLogix.Entity.DTOs.CouncilDTOs
{
    public class VoteOperationDto
    {
        public int ProtocolId { get; set; }
        public int RefereeDoctorId { get; set; } // Oy veren kişi (Şimdilik manuel seçeceğiz)
        public bool VoteResult { get; set; }     // true: Kabul, false: Red
        public string Comment { get; set; }      // Şerh / Açıklama
    }
}