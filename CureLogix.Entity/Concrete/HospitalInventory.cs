namespace CureLogix.Entity.Concrete
{
    public class HospitalInventory
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public int Type { get; set; } // 1:Temiz, 2:Atık
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastUpdated { get; set; }

        public virtual Hospital Hospital { get; set; }
        public virtual Medicine Medicine { get; set; }
    }
}