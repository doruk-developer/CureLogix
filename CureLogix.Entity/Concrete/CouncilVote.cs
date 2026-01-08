namespace CureLogix.Entity.Concrete
{
    public class CouncilVote
    {
        public int Id { get; set; }
        public int ProtocolId { get; set; }
        public int RefereeDoctorId { get; set; }
        public bool Vote { get; set; }
        public string Comment { get; set; }
        public DateTime? VoteDate { get; set; }

        public virtual TreatmentProtocol Protocol { get; set; }
        public virtual Doctor RefereeDoctor { get; set; }
    }
}
