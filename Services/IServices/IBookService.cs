using BookLibraryApi.Models;

namespace BookLibraryApp.Services.IServices
{
    public interface IBookService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id, string token);
        Task<T> CreateAsync<T>(Book book, string token);
        Task<T> UpdateAsync<T>(Book book, string token);
        Task<T> DeleteAsync<T>(int id, string token);
        Task<T> GetGenres<T>();
    }
}
