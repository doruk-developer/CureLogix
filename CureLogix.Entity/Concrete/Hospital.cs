namespace CureLogix.Entity.Concrete
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int MainStorageCapacity { get; set; }
        public int WasteStorageCapacity { get; set; }
        public decimal? OccupancyRate { get; set; }
        public bool? IsActive { get; set; }

        // İlişkiler
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public virtual ICollection<HospitalInventory> HospitalInventories { get; set; } = new List<HospitalInventory>();
        public virtual ICollection<SupplyRequest> SupplyRequests { get; set; } = new List<SupplyRequest>();
    }
}
