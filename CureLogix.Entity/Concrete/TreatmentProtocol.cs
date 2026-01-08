namespace CureLogix.Entity.Concrete
{
    public class TreatmentProtocol
    {
        public int Id { get; set; }
        public int DiseaseId { get; set; }
        public int DoctorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; } // 0:Bekliyor, 1:Onay, 2:Red
        public DateTime? CreatedDate { get; set; }

        public virtual Disease Disease { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual ICollection<ProtocolMedicine> ProtocolMedicines { get; set; } = new List<ProtocolMedicine>();
        public virtual ICollection<CouncilVote> CouncilVotes { get; set; } = new List<CouncilVote>();
    }
}