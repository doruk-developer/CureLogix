using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;     // Doctor sınıfı için

namespace CureLogix.Business.Concrete
{
    public class DoctorManager : GenericManager<Doctor>, IDoctorService
    {
        public DoctorManager(IGenericRepository<Doctor> repository) : base(repository) { }
    }
}