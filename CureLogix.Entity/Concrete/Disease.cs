namespace CureLogix.Entity.Concrete
{
    public class Disease
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string RiskLevel { get; set; }

        public virtual ICollection<TreatmentProtocol> TreatmentProtocols { get; set; } = new List<TreatmentProtocol>();
    }
}