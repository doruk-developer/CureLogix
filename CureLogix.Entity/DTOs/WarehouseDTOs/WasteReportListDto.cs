using System;

namespace CureLogix.Entity.DTOs.WasteDTOs
{
    public class WasteReportListDto
    {
        public int Id { get; set; }
        public string HospitalName { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public DateTime DisposalDate { get; set; }
        public string Reason { get; set; }
    }
}