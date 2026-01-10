namespace CureLogix.Entity.DTOs.MapDTOs
{
    public class MapPointDto
    {
        public string HospitalName { get; set; }
        public string City { get; set; }
        public double Latitude { get; set; }  // Enlem
        public double Longitude { get; set; } // Boylam
        public decimal OccupancyRate { get; set; } // Doluluk (Rengi belirleyecek)
        public string LevelColor { get; set; } // #FF0000 gibi renk kodu
    }
}