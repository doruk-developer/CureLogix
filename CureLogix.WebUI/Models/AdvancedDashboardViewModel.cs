using CureLogix.Entity.DTOs.WarehouseDTOs;

namespace CureLogix.WebUI.Models
{
    // : DashboardViewModel diyerek mirası aldık
    public class AdvancedDashboardViewModel : DashboardViewModel
    {
        public AdvancedDashboardViewModel()
        {
            // Listeleri boş başlat (Null hatası almamak için)
            HospitalNames = new List<string>();
            OccupancyRates = new List<decimal>();
            MedicineCategories = new List<string>();
            CategoryStockLevels = new List<int>();
            CriticalMedicines = new List<CentralStockListDto>();
            RecentActivities = new List<string>();
        }

        // --- DİKKAT: KPI (TotalHospitals vb.) BURAYA YAZILMAZ! ---
        // Onlar DashboardViewModel'den otomatik geliyor.

        // 2. Grafikler
        public List<string> HospitalNames { get; set; }
        public List<decimal> OccupancyRates { get; set; }

        public int ApprovedReq { get; set; }
        public int WaitingReq { get; set; }
        public int RejectedReq { get; set; }

        // 3. Radar Grafik
        public List<string> MedicineCategories { get; set; }
        public List<int> CategoryStockLevels { get; set; }

        // 4. Tablolar
        public List<CentralStockListDto> CriticalMedicines { get; set; }
        public List<string> RecentActivities { get; set; }
    }
}