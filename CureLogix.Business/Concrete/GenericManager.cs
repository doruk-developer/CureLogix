using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;

namespace CureLogix.Business.Concrete
{
    public class GenericManager<T> : IGenericService<T> where T : class
    {
        protected readonly IGenericRepository<T> _repository;

        public GenericManager(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public void TAdd(T t)
        {
            _repository.Insert(t);
        }

        public void TDelete(T t)
        {
            _repository.Delete(t);
        }

        public T TGetById(int id)
        {
            return _repository.GetById(id);
        }

        public List<T> TGetList()
        {
            return _repository.GetList();
        }

        public void TUpdate(T t)
        {
            _repository.Update(t);
        }

        // Server-Side Datatables Modülü için
        public IQueryable<T> GetQuery()
        {
            // Business katmanı, işi Repository'e devreder
            return _repository.GetQuery();
        }
    }
}