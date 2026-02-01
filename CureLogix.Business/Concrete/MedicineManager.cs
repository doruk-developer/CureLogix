using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;
using CureLogix.Entity.DTOs.SearchDTOs;

namespace CureLogix.Business.Concrete
{
    public class MedicineManager : GenericManager<Medicine>, IMedicineService
    {
        private readonly IElasticSearchService _elasticService;

        // Constructor'da ElasticService'i içeri alıyoruz (Dependency Injection)
        public MedicineManager(IGenericRepository<Medicine> repository, IElasticSearchService elasticService)
            : base(repository)
        {
            _elasticService = elasticService;
        }

        // EKLEME İŞLEMİ (Override)
        public override void TAdd(Medicine t)
        {
            // 1. Önce SQL'e kaydet (ID oluşsun)
            base.TAdd(t);

            // 2. Sonra Elasticsearch'e gönder (Fire & Forget)
            // Arka planda çalışır, kullanıcıyı bekletmez.
            var searchModel = new MedicineSearchModel
            {
                Id = t.Id,
                Name = t.Name,
                ActiveIngredient = t.ActiveIngredient,
                Unit = t.Unit,
                StockQuantity = 0, // Yeni eklenen ilacın stoğu başta 0'dır
                IsCritical = false,
                RequiresColdChain = t.RequiresColdChain ?? false
            };

            // Asenkron metodu senkron içinde çağırmak için basit bir trick:
            Task.Run(() => _elasticService.IndexMedicineAsync(searchModel));
        }

        // GÜNCELLEME İŞLEMİ (Override)
        public override void TUpdate(Medicine t)
        {
            // 1. SQL'i güncelle
            base.TUpdate(t);

            // 2. Elasticsearch'ü güncelle
            var searchModel = new MedicineSearchModel
            {
                Id = t.Id,
                Name = t.Name,
                ActiveIngredient = t.ActiveIngredient,
                Unit = t.Unit,
                // Stok miktarını veritabanından çekmek gerekebilir ama şimdilik entity'den alıyoruz
                // Not: Gerçek stok 'CentralWarehouse' tablosundadır, burası ilaç tanımıdır.
                StockQuantity = 0,
                IsCritical = (t.CriticalStockLevel > 0),
                RequiresColdChain = t.RequiresColdChain ?? false
            };

            Task.Run(() => _elasticService.IndexMedicineAsync(searchModel));
        }
    }
}