using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class DiseaseManager : GenericManager<Disease>, IDiseaseService
    {
        public DiseaseManager(IGenericRepository<Disease> repository) : base(repository) { }
    }
}