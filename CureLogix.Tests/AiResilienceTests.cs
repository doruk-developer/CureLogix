using CureLogix.Business.Concrete;
using Xunit;

namespace CureLogix.Tests
{
    public class AiResilienceTests
    {
        [Fact]
        public void Forecast_Should_Return_Fallback_Value_If_Model_Fails()
        {
            // 1. ARRANGE
            // AI Manager'ı başlatıyoruz (Gerçek ML dosyasını bulamazsa hata atar, biz de bunu test ediyoruz)
            // Not: Bu testin geçmesi için AiForecastManager içindeki try-catch bloğunun çalışması lazım.
            var aiManager = new AiForecastManager();

            // 2. ACT
            // Bilerek çok saçma veya hatalı bir istekte bulunalım veya dosya yolu yokken deneyelim.
            // Amaç: Metodun "throw" yapmadan bize bir sayı dönmesi.
            var result = aiManager.PredictDemand("BilinmeyenSehir", "BilinmeyenIlac", 13); // 13. Ay yok :)

            // 3. ASSERT
            // Sonuç null olmamalı.
            Assert.NotNull(result);

            // Sonuç eksi olmamalı (Negatif talep olamaz).
            Assert.True(result.PredictedDemand >= 0);

            // Sonuç bir sayı olmalı (Sistem çökmemeli).
            Assert.IsType<float>(result.PredictedDemand);
        }
    }
}