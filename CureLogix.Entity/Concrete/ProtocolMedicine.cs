namespace CureLogix.Entity.Concrete
{
    public class ProtocolMedicine
    {
        public int Id { get; set; }
        public int ProtocolId { get; set; }
        public int MedicineId { get; set; }
        public int RequiredQuantity { get; set; }
        public string DosageInstructions { get; set; }

        public virtual TreatmentProtocol Protocol { get; set; }
        public virtual Medicine Medicine { get; set; }
    }
}
