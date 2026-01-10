using System;

namespace CureLogix.Entity.Concrete
{
    public class WasteReport
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public DateTime DisposalDate { get; set; }
        public string Reason { get; set; }
        public string AuthorizedPerson { get; set; }

        public virtual Hospital Hospital { get; set; }
        public virtual Medicine Medicine { get; set; }
    }
}