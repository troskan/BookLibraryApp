using BookLibraryApi.Models;
using BookLibraryApp.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace BookLibraryApp.Services.IServices
{
    public interface IBaseService
    {
        ApiResponse responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
