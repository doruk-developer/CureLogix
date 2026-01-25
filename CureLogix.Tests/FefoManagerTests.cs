using CureLogix.Business.Concrete;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;
using Moq;
using System.Linq.Expressions;

namespace CureLogix.Tests
{
    public class FefoManagerTests
    {
        [Fact] // Bu bir test metodudur etiketi
        public void DistributeStockByFEFO_Should_Pick_Closest_Expiry_Date_First()
        {
            // 1. ARRANGE (Hazırlık)
            // Sahte bir depo (Repository) oluşturuyoruz.
            var mockRepo = new Mock<IGenericRepository<CentralWarehouse>>();

            // Senaryo: 101 ID'li İlaçtan 2 parti var.
            // Parti A: SKT'si YARIN, Stok: 50 Adet (Önce bu bitmeli!)
            // Parti B: SKT'si SENEYE, Stok: 100 Adet
            var fakeStocks = new List<CentralWarehouse>
            {
                new CentralWarehouse { Id = 1, MedicineId = 101, Quantity = 50, ExpiryDate = DateTime.Now.AddDays(1) },
                new CentralWarehouse { Id = 2, MedicineId = 101, Quantity = 100, ExpiryDate = DateTime.Now.AddYears(1) }
            };

            // Repo'ya "GetListByFilter çağrılırsa bu sahte listeyi ver" diyoruz.
            mockRepo.Setup(x => x.GetListByFilter(It.IsAny<Expression<Func<CentralWarehouse, bool>>>()))
                    .Returns(fakeStocks);

            // Test edeceğimiz Manager'ı, sahte repo ile başlatıyoruz.
            var manager = new CentralWarehouseManager(mockRepo.Object);

            // 2. ACT (Eylem)
            // 60 adet ürün istiyoruz. (50 tane A'dan, 10 tane B'den almalı)
            manager.DistributeStockByFEFO(101, 60);

            // 3. ASSERT (Doğrulama)
            // Parti A (Yarın ölecek olan) tamamen bitmiş olmalı (0 kalmalı).
            Assert.Equal(0, fakeStocks[0].Quantity);

            // Parti B (Seneye ölecek olan) 100 - 10 = 90 kalmalı.
            Assert.Equal(90, fakeStocks[1].Quantity);
        }
    }
}