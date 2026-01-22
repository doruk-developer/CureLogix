using CureLogix.Entity.DTOs.HospitalDTOs;
using CureLogix.WebUI.Models;

namespace CureLogix.WebUI.Helpers
{
    public static class DummyDataGenerator
    {
        public static AdvancedDashboardViewModel GetDashboard()
        {
            return new AdvancedDashboardViewModel
            {
                // Temel Veriler (Babadan gelenler)
                TotalHospitals = 12,
                TotalDoctors = 48,
                PendingRequests = 15,
                CriticalStockCount = 7,

                // Gelişmiş Veriler
                HospitalNames = new List<string> { "Merkez", "Şehir Has.", "Tıp Fak.", "Onkoloji", "Çocuk Has.", "Acil Durum" },
                OccupancyRates = new List<decimal> { 85, 45, 92, 60, 30, 75 },

                ApprovedReq = 120,
                WaitingReq = 45,
                RejectedReq = 12,

                MedicineCategories = new List<string> { "Antibiyotik", "Antiviral", "Analjezik", "Kardiyak", "Solunum" },
                CategoryStockLevels = new List<int> { 90, 60, 85, 40, 70 },

                RecentActivities = new List<string>
                {
                    "Dr. Ali sisteme giriş yaptı.",
                    "Merkez Depo'ya 50.000 Kutu Maske eklendi.",
                    "Ankara Şehir Hastanesi acil oksijen talep etti.",
                    "Sistem yedeği başarıyla alındı."
                },

                CriticalMedicines = new List<CureLogix.Entity.DTOs.WarehouseDTOs.CentralStockListDto>
                {
                    new() { MedicineName = "Adrenalin 1mg", Quantity = 5, BatchNo = "TEST-001" },
                    new() { MedicineName = "Morfin HCL", Quantity = 12, BatchNo = "TEST-002" },
                    new() { MedicineName = "Insulin R", Quantity = 0, BatchNo = "TEST-003" }
                }
            };
        }

        public static List<HospitalListDto> GetHospitals()
        {
            return new List<HospitalListDto>
            {
                new() { Id = 1, Name = "[DEMO] Ulusal Tıp Merkezi", City = "İstanbul", MainStorageCapacity = 100000 },
                new() { Id = 2, Name = "[DEMO] Bölge Eğitim Araştırma", City = "Ankara", MainStorageCapacity = 85000 },
                new() { Id = 3, Name = "[DEMO] Sahra Hastanesi - 1", City = "Hatay", MainStorageCapacity = 20000 }
            };
        }
    }
}