using CureLogix.Business.Abstract;
using CureLogix.Entity.AI;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;

namespace CureLogix.Business.Concrete
{
    public class AiForecastManager : IAiForecastService
    {
        private static string _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CureLogixModel.zip");
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;

        public AiForecastManager()
        {
            _mlContext = new MLContext(seed: 0); // Seed 0: Her seferinde aynı sonucu üret (Deterministik)

            // Uygulama başlarken modeli eğit veya yükle
            TrainModel();
        }

        public void TrainModel()
        {
            // 1. ADIM: SENTETİK EĞİTİM VERİSİ OLUŞTURMA (Simülasyon)
            // Gerçekte bu veriler veritabanından (SupplyRequests tablosundan) gelir.
            var data = new List<MedicineDemandInput>();
            var cities = new[] { "İstanbul", "Ankara", "İzmir", "Antalya", "Trabzon", "Erzurum" };
            var medicines = new[] { "ViruGuard 500mg", "Parol 500mg", "ImmunoZinc Komplex" };
            var rnd = new Random();

            // Son 3 yılın verisini simüle et
            for (int i = 0; i < 1000; i++)
            {
                var city = cities[rnd.Next(cities.Length)];
                var medicine = medicines[rnd.Next(medicines.Length)];
                var month = rnd.Next(1, 13);

                // --- İŞİN SIRRI BURADA: MANTIKLI VERİ ÜRETME ---
                float baseDemand = 100;

                // Kış ayları (12, 1, 2) ve Sonbahar (9, 10, 11) talep artar
                if (month < 3 || month > 9) baseDemand += 200;

                // Büyük şehirlerde talep daha fazladır
                if (city == "İstanbul" || city == "Ankara") baseDemand += 150;

                // Antalya yazın kalabalık olur
                if (city == "Antalya" && (month > 5 && month < 9)) baseDemand += 300;

                // Rastgelelik ekle (Gürültü)
                float noise = rnd.Next(-50, 50);

                data.Add(new MedicineDemandInput
                {
                    City = city,
                    MedicineName = medicine,
                    Month = month,
                    ActualDemand = baseDemand + noise
                });
            }

            // Veriyi IDataView formatına çevir
            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            // 2. ADIM: ML PIPELINE OLUŞTURMA (Öğrenme Hattı)
            var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "ActualDemand")
                // Şehir ve İlaç isimlerini Sayıya (Vektöre) çevir (OneHotEncoding)
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding("CityEncoded", "City"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding("MedicineEncoded", "MedicineName"))
                // Özellikleri birleştir
                .Append(_mlContext.Transforms.Concatenate("Features", "Month", "CityEncoded", "MedicineEncoded"))
                // Algoritma Seçimi: FastTree (Karar Ağaçları - Regresyon için çok güçlüdür)
                .Append(_mlContext.Regression.Trainers.FastTree());

            // 3. ADIM: MODELİ EĞİT (Train)
            _trainedModel = pipeline.Fit(trainingData);

            // Modeli dosyaya kaydet (İsteğe bağlı, performans için)
            _mlContext.Model.Save(_trainedModel, trainingData.Schema, _modelPath);
        }

        public MedicineDemandPrediction PredictDemand(string city, string medicineName, int month)
        {
            // Tahmin Motorunu Oluştur
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MedicineDemandInput, MedicineDemandPrediction>(_trainedModel);

            // Girdiyi Hazırla
            var input = new MedicineDemandInput
            {
                City = city,
                MedicineName = medicineName,
                Month = month
            };

            // Tahmin Yap
            var result = predictionEngine.Predict(input);

            // Eksi değer çıkarsa 0 yap (Regresyon bazen eksiye düşebilir)
            if (result.PredictedDemand < 0) result.PredictedDemand = 0;

            return result;
        }
    }
}