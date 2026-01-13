using System.Linq.Expressions;

namespace CureLogix.DataAccess.Abstract
{
    public interface IGenericRepository<T> where T : class
    {
        void Insert(T t);
        void Delete(T t);
        void Update(T t);
        List<T> GetList();
        T? GetById(int id);
        List<T> GetListByFilter(Expression<Func<T, bool>> filter);
        // Server-Side Datatables Modülü için
        IQueryable<T> GetQuery();
    }
}