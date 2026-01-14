using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class DoctorManager : GenericManager<Doctor>, IDoctorService
    {
        public DoctorManager(IGenericRepository<Doctor> repository) : base(repository)
        {
        }
        // Burada "public override void TUpdate..." gibi bir kod Varsa SİL!
        // Base sınıftaki (GenericManager) doğru metodu kullansın.
    }
}