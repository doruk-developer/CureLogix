using CureLogix.DataAccess.Abstract;
using CureLogix.DataAccess.Concrete; // Context burada
using System.Linq.Expressions;

namespace CureLogix.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CureLogixContext _context;

        public GenericRepository(CureLogixContext context)
        {
            _context = context;
        }

        public void Delete(T t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public List<T> GetList()
        {
            return _context.Set<T>().ToList();
        }

        public List<T> GetListByFilter(Expression<Func<T, bool>> filter)
        {
            return _context.Set<T>().Where(filter).ToList();
        }

        public void Insert(T t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(T t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}