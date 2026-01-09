using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class SupplyRequestManager : GenericManager<SupplyRequest>, ISupplyRequestService
    {
        public SupplyRequestManager(IGenericRepository<SupplyRequest> repository) : base(repository) { }
    }
}