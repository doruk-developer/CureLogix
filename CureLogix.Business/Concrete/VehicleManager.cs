using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class VehicleManager : GenericManager<Vehicle>, IVehicleService
    {
        public VehicleManager(IGenericRepository<Vehicle> repository) : base(repository) { }
    }
}