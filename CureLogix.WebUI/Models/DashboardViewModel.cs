namespace CureLogix.WebUI.Models
{
    public class DashboardViewModel
    {
        public int TotalHospitals { get; set; }
        public int TotalDoctors { get; set; }
        public int PendingRequests { get; set; }
        public int CriticalStockCount { get; set; }
    }
}