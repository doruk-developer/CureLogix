using CureLogix.Business.Abstract;
using CureLogix.Entity.DTOs.MapDTOs;
using Microsoft.AspNetCore.Mvc;

namespace CureLogix.WebUI.Controllers
{
    public class MapController : Controller
    {
        private readonly IHospitalService _hospitalService;

        public MapController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Harita verisini JSON olarak dönecek metot (AJAX ile çağıracağız)
        [HttpGet]
        public IActionResult GetMapData()
        {
            var hospitals = _hospitalService.TGetList();
            var mapPoints = new List<MapPointDto>();

            foreach (var item in hospitals)
            {
                // 1. Koordinatları Şehre Göre Ata (Simülasyon)
                var coords = GetCoordinates(item.City);

                // 2. Rengi Belirle (Doluluk oranına göre)
                string color = "#28a745"; // Yeşil (Düşük)
                if (item.OccupancyRate > 75) color = "#dc3545"; // Kırmızı (Kritik)
                else if (item.OccupancyRate > 50) color = "#ffc107"; // Sarı (Orta)

                mapPoints.Add(new MapPointDto
                {
                    HospitalName = item.Name,
                    City = item.City,
                    OccupancyRate = item.OccupancyRate ?? 0,
                    Latitude = coords.lat,
                    Longitude = coords.lng,
                    LevelColor = color
                });
            }

            return Json(mapPoints);
        }

        // Basit Koordinat Sözlüğü (Gerçek projede DB'den gelir)
        private (double lat, double lng) GetCoordinates(string city)
        {
            return city switch
            {
                "İstanbul" => (41.0082, 28.9784),
                "Ankara" => (39.9334, 32.8597),
                "İzmir" => (38.4237, 27.1428),
                "Antalya" => (36.8969, 30.7133),
                "Trabzon" => (41.0027, 39.7168),
                "Erzurum" => (39.9043, 41.2679),
                _ => (39.0, 35.0) // Bilinmeyen şehirler için Türkiye ortası
            };
        }
    }
}