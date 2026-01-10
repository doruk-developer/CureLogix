namespace CureLogix.Entity.Concrete
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public string DriverName { get; set; }
        public bool HasCoolingSystem { get; set; } // IoT Sensör Simülasyonu
        public bool IsActive { get; set; }

        public virtual ICollection<SupplyRequest> SupplyRequests { get; set; }
    }
}