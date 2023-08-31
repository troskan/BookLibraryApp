using BookLibraryApi.Models;
using BookLibraryApp.Services.IServices;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace BookLibraryApp.Services
{
    public class BookService : BaseService, IBookService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string bookUrl;
        public BookService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            bookUrl = configuration.GetValue<string>("ServiceUrls:BookApi");
        }
        public Task<T> CreateAsync<T>(Book book, string token)
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = Utility.ApiUtility.ApiType.POST,
                Data = book,
                Url = bookUrl + "/book",
                Token = ""
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = Utility.ApiUtility.ApiType.DELETE,
                Url = bookUrl + "/book/" + id,
                Token = ""
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = Utility.ApiUtility.ApiType.GET,
                Url = bookUrl + "/book",
                Token = ""
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = Utility.ApiUtility.ApiType.GET,
                Url = bookUrl + "/book/" + id,
                Token= ""
            });
        }

        public Task<T> UpdateAsync<T>(Book book, string token)
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = Utility.ApiUtility.ApiType.PUT,
                Data = book,
                Url = bookUrl + "/book/" + book.BookId

            });
        }
        public Task<T> GetGenres<T>()
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = Utility.ApiUtility.ApiType.GET,
                Url = bookUrl + "/genres",
                Token = ""
            });
        }
        public Task<T> Search<T>(string searchString)
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = Utility.ApiUtility.ApiType.GET,
                Url = bookUrl + "/search?searchString=" + searchString,
                Token = ""
            });
        }
    }
}
