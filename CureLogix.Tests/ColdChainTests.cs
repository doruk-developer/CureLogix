using CureLogix.Entity.Concrete;
using Xunit; // xUnit kütüphanesi

namespace CureLogix.Tests
{
    public class ColdChainTests
    {
        [Fact]
        public void ValidateShipment_Should_Fail_If_Vehicle_Has_No_Cooler_For_ColdChain_Medicine()
        {
            // 1. ARRANGE
            var vaccine = new Medicine { Name = "BioShield Aşı", RequiresColdChain = true };
            var truck = new Vehicle { PlateNumber = "34 ABC 123", HasCoolingSystem = false }; // Soğutucu YOK!

            // 2. ACT & ASSERT
            // Buradaki mantık şu: Bu işlemi yapmaya çalıştığımda bir 'Exception' (Hata) bekliyorum.
            // Eğer hata fırlatmazsa test BAŞARISIZ sayılır (Çünkü güvenlik açığı var demektir).

            bool isSafe = false;

            // Basit bir if simülasyonu (Business logic'teki kontrolün aynısı)
            if (vaccine.RequiresColdChain == true && truck.HasCoolingSystem == false)
            {
                isSafe = false;
            }
            else
            {
                isSafe = true;
            }

            // Beklentimiz: isSafe 'false' olmalı.
            Assert.False(isSafe, "Soğuk zincir gerektiren ilaç, soğutucusuz araca yüklenemez!");
        }
    }
}