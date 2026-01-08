namespace CureLogix.WebUI.Models
{
    public class DashboardViewModel
    {
        // Kartlar için Sayılar
        public int TotalHospitals { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalMedicines { get; set; }
        public int TotalProtocols { get; set; } // Yeni eklendi

        // Grafik 1: Hastane Doluluk (İsimler ve Oranlar)
        public List<string> HospitalNames { get; set; }
        public List<decimal> OccupancyRates { get; set; }

        // Grafik 2: Protokol Durumları (Onay, Red, Bekleyen sayıları)
        public int ApprovedProtocols { get; set; }
        public int RejectedProtocols { get; set; }
        public int PendingProtocols { get; set; }

        // Grafik 3: Doktor Branşları
        public List<string> DoctorSpecialties { get; set; }
        public List<int> DoctorSpecialtyCounts { get; set; }
    }
}