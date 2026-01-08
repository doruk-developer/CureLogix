namespace CureLogix.Entity.Concrete
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ActiveIngredient { get; set; }
        public string Unit { get; set; }
        public int ShelfLifeDays { get; set; }
        public int? CriticalStockLevel { get; set; }
        public bool? RequiresColdChain { get; set; }

        // İlişkiler
        public virtual ICollection<ProtocolMedicine> ProtocolMedicines { get; set; } = new List<ProtocolMedicine>();
        public virtual ICollection<CentralWarehouse> CentralWarehouses { get; set; } = new List<CentralWarehouse>();
        public virtual ICollection<HospitalInventory> HospitalInventories { get; set; } = new List<HospitalInventory>();
        public virtual ICollection<SupplyRequest> SupplyRequests { get; set; } = new List<SupplyRequest>();
    }
}