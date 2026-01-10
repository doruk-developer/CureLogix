namespace CureLogix.Entity.Concrete
{
    public class SupplyRequest
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public int RequestingDoctorId { get; set; }
        public int MedicineId { get; set; }
        public int RequestQuantity { get; set; }
        public int? ApprovedQuantity { get; set; }
        public int? Status { get; set; }
        public DateTime? RequestDate { get; set; }

        public virtual Hospital Hospital { get; set; }
        public virtual Doctor RequestingDoctor { get; set; }
        public virtual Medicine Medicine { get; set; }

        // Soğuk zincir araçları için
        public int? AssignedVehicleId { get; set; }
        public virtual Vehicle AssignedVehicle { get; set; }
    }
}