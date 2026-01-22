using CureLogix.DataAccess.Abstract;
using CureLogix.DataAccess.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            _context.SaveChanges(); // Silme için kayıt
        }

        public T? GetById(int id)
        {
            Console.WriteLine($"========== GET BY ID ==========");
            Console.WriteLine($"Aranan ID: {id}");
            Console.WriteLine($"Entity Type: {typeof(T).Name}");

            var entity = _context.Set<T>().Find(id);

            Console.WriteLine($"Kayıt bulundu mu? {entity != null}");

            if (entity != null)
            {
                var entry = _context.Entry(entity);
                Console.WriteLine($"Entity Tracking State: {entry.State}");
            }

            return entity;
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
            _context.SaveChanges(); // Ekleme için kayıt
        }

        public void Update(T t)
        {
            Console.WriteLine($"========== GENERIC REPOSITORY UPDATE ==========");
            Console.WriteLine($"Entity Type: {typeof(T).Name}");
            Console.WriteLine($"Entity NULL mu? {t == null}");

            // 1. ADIM: Eğer t null ise işlem yapma, direk dön (Güvenlik Önlemi)
            if (t == null)
            {
                Console.WriteLine("HATA: Güncellenecek entity NULL geldi, işlem iptal edildi.");
                return;
            }

            try
            {
                // 2. ADIM: t'nin yanına '!' koyarak derleyiciye "Merak etme bu null değil" diyoruz.
                var entry = _context.Entry(t!);

                Console.WriteLine($"Entry State (Önce): {entry.State}");

                if (entry.State == EntityState.Detached)
                {
                    Console.WriteLine("Entity DETACHED durumda, Attach ediliyor...");

                    // Burada da '!' kullanıyoruz (CS8604 uyarısı için)
                    _context.Set<T>().Attach(t!);
                }

                entry.State = EntityState.Modified;
                Console.WriteLine($"Entry State (Sonra): {entry.State}");

                Console.WriteLine("SaveChanges() çağrılıyor...");
                int affectedRows = _context.SaveChanges();
                Console.WriteLine($"Etkilenen Satır Sayısı: {affectedRows}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"REPOSITORY UPDATE HATASI: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                throw;
            }
        }

        public IQueryable<T> GetQuery()
        {
            return _context.Set<T>().AsQueryable();
        }
    }
}