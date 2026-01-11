using Microsoft.ML.Data;

namespace CureLogix.Entity.AI
{
    public class MedicineDemandPrediction
    {
        [ColumnName("Score")] // ML.NET Regresyon sonucu "Score" kolonuna yazar
        public float PredictedDemand { get; set; }
    }
}