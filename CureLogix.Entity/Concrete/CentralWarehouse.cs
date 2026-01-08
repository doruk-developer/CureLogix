namespace CureLogix.Entity.Concrete
{
    public class CentralWarehouse
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual Medicine Medicine { get; set; }
    }
}