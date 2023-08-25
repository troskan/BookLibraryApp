namespace BookLibraryApp.Services.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(T entity);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
    }
}
