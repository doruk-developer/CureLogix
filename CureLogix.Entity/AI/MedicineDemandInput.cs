using Microsoft.ML.Data;

namespace CureLogix.Entity.AI
{
    public class MedicineDemandInput
    {
        [LoadColumn(0)] public float Month { get; set; }      // Hangi Ay? (1-12)
        [LoadColumn(1)] public string City { get; set; }      // Hangi Şehir?
        [LoadColumn(2)] public string MedicineName { get; set; } // Hangi İlaç?
        [LoadColumn(3)] public float ActualDemand { get; set; } // Gerçekleşen Talep (Label)
    }
}