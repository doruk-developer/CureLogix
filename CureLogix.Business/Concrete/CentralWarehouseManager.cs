using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class CentralWarehouseManager : GenericManager<CentralWarehouse>, ICentralWarehouseService
    {
        public CentralWarehouseManager(IGenericRepository<CentralWarehouse> repository) : base(repository) { }
    }
}