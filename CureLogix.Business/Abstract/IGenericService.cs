namespace CureLogix.Business.Abstract
{
    public interface IGenericService<T>
    {
        void TAdd(T t);
        void TDelete(T t);
        void TUpdate(T t);
        List<T> TGetList();
        T TGetById(int id);
        // --- Server-Side Datatables Modülü için ---
        IQueryable<T> GetQuery();
    }
}