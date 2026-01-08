using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class HospitalManager : GenericManager<Hospital>, IHospitalService
    {
        public HospitalManager(IGenericRepository<Hospital> repository) : base(repository) { }
    }
}