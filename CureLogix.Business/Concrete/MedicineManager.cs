using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class MedicineManager : GenericManager<Medicine>, IMedicineService
    {
        public MedicineManager(IGenericRepository<Medicine> repository) : base(repository) { }
    }
}