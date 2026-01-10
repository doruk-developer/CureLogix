using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;
using System;
using System.Linq;

namespace CureLogix.Business.Concrete
{
    public class WasteReportManager : GenericManager<WasteReport>, IWasteReportService
    {
        private readonly IGenericRepository<HospitalInventory> _inventoryRepository;

        public WasteReportManager(IGenericRepository<WasteReport> repository, IGenericRepository<HospitalInventory> inventoryRepository) : base(repository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public void MoveExpiredToWaste()
        {
            // 1. SKT'si geçmiş ve hala 'Temiz Stok (Type: 1)' olanları bul
            var expiredItems = _inventoryRepository.GetListByFilter(x => x.ExpiryDate < DateTime.Now && x.Type == 1);

            foreach (var item in expiredItems)
            {
                item.Type = 2; // Statüyü 'Tıbbi Atık' yap
                _inventoryRepository.Update(item);
            }
        }
    }
}