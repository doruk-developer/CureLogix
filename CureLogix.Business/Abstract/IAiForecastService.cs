using CureLogix.Entity.AI;

namespace CureLogix.Business.Abstract
{
    public interface IAiForecastService
    {
        // Gelecek ay için tahmin yapar
        MedicineDemandPrediction PredictDemand(string city, string medicineName, int month);

        // Modeli eğitir (Arka planda)
        void TrainModel();
    }
}