using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class CentralWarehouseManager : GenericManager<CentralWarehouse>, ICentralWarehouseService
    {
        public CentralWarehouseManager(IGenericRepository<CentralWarehouse> repository) : base(repository) { }

        public void DistributeStockByFEFO(int medicineId, int quantityNeeded)
        {
            // 1. ADIM: İlgili ilacın tüm stoklarını getir
            // Kritik Nokta: OrderBy(ExpiryDate) ile SKT'si en yakın olanı en başa alıyoruz! (FEFO)
            var stocks = _repository.GetListByFilter(x => x.MedicineId == medicineId)
                                    .OrderBy(x => x.ExpiryDate)
                                    .ToList();

            // Stok yetiyor mu kontrolü (Basitçe toplam stok)
            if (stocks.Sum(x => x.Quantity) < quantityNeeded)
            {
                throw new Exception("Merkez depoda yeterli stok yok!"); // İleride özel Exception yapılır
            }

            // 2. ADIM: Dağıtım Döngüsü
            foreach (var batch in stocks)
            {
                if (quantityNeeded <= 0) break; // İhtiyaç bittiyse döngüden çık

                if (batch.Quantity <= quantityNeeded)
                {
                    // Senaryo A: Bu partideki stok, ihtiyacın tamamını veya bir kısmını karşılıyor ama bitiyor.
                    // Örn: İhtiyaç 5000, Batch A'da 2000 var.

                    quantityNeeded -= batch.Quantity; // İhtiyaçtan 2000 düş (Kaldı 3000)
                    batch.Quantity = 0; // Bu parti bitti

                    // Not: Gerçek hayatta kaydı silmeyiz, 0 olarak bırakırız veya Log atarız.
                    _repository.Update(batch);
                }
                else
                {
                    // Senaryo B: Bu partideki stok, ihtiyacı fazlasıyla karşılıyor.
                    // Örn: İhtiyaç 3000 kaldı, Batch B'de 10.000 var.

                    batch.Quantity -= quantityNeeded; // Stoktan 3000 düş (Kaldı 7000)
                    quantityNeeded = 0; // İhtiyaç bitti
                    _repository.Update(batch);
                }
            }
        }
    }
}