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

        public virtual void TAdd(T t)
        {
            _repository.Insert(t);
        }

        public virtual void TDelete(T t)
        {
            _repository.Delete(t);
        }

        public virtual T TGetById(int id)
        {
            // ✅ DÜZELTME: Sonuna '!' koyarak null uyarısını susturduk.
            return _repository.GetById(id)!;
        }

        public virtual List<T> TGetList()
        {
            return _repository.GetList();
        }

        public virtual void TUpdate(T t)
        {
            _repository.Update(t);
        }

        public IQueryable<T> GetQuery()
        {
            return _repository.GetQuery();
        }
    }
}