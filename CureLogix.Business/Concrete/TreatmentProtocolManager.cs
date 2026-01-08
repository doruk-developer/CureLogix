using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class TreatmentProtocolManager : GenericManager<TreatmentProtocol>, ITreatmentProtocolService
    {
        public TreatmentProtocolManager(IGenericRepository<TreatmentProtocol> repository) : base(repository) { }
    }
}