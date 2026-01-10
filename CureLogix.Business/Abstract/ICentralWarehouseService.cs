using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Abstract
{
    public interface ICentralWarehouseService : IGenericService<CentralWarehouse>
    {
        // Yeni Metot: Akıllı Stok Düşümü
        void DistributeStockByFEFO(int medicineId, int quantityNeeded);

        // Arkaplan görevleri için
        void CheckExpiriesAndCreateOrder();
    }
}