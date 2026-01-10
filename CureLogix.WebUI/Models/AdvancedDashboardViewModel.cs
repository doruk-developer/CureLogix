using CureLogix.Entity.DTOs.SupplyDTOs;
using CureLogix.Entity.DTOs.WarehouseDTOs;

namespace CureLogix.WebUI.Models
{
    public class AdvancedDashboardViewModel
    {
        // 1. KPI (Anahtar Performans Göstergeleri)
        public int TotalHospitals { get; set; }
        public int TotalDoctors { get; set; }
        public int PendingRequests { get; set; } // Bekleyen Talepler
        public int CriticalStockCount { get; set; } // Kırmızı Alarm veren ilaç sayısı

        // 2. Grafik Verileri
        // A) Hastane Doluluk (Bar Chart)
        public List<string> HospitalNames { get; set; }
        public List<decimal> OccupancyRates { get; set; }

        // B) Talep Durumları (Doughnut Chart)
        public int ApprovedReq { get; set; }
        public int RejectedReq { get; set; }
        public int WaitingReq { get; set; }

        // C) Stok Değer Analizi (Radar Chart - Simülasyon)
        // Hangi kategoride ne kadar güçlüyüz? (Antibiyotik, Aşı, Ağrı Kesici vb.)
        public List<string> MedicineCategories { get; set; }
        public List<int> CategoryStockLevels { get; set; }

        // 3. Tablolar
        public List<CentralStockListDto> CriticalMedicines { get; set; } // Bitenler
        public List<string> RecentActivities { get; set; } // Son İşlemler (Timeline)
    }
}