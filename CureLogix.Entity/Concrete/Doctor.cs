namespace CureLogix.Entity.Concrete
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Specialty { get; set; }
        public int RoleType { get; set; } // 1:Saha, 2:Hakem, 3:Başhekim
        public int? HospitalId { get; set; }
        public string? Email { get; set; }

        // İlişkiler
        public virtual Hospital Hospital { get; set; }
        public virtual ICollection<TreatmentProtocol> TreatmentProtocols { get; set; } = new List<TreatmentProtocol>();
        public virtual ICollection<CouncilVote> CouncilVotes { get; set; } = new List<CouncilVote>();
        public virtual ICollection<SupplyRequest> SupplyRequests { get; set; } = new List<SupplyRequest>();
    }
}