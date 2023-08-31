using BookLibraryApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace BookLibraryApp.Services
{
    public class BookApiService
    {
        private readonly IHttpClientFactory _iHttp;
        private readonly ILogger _logger;
        public BookApiService(IHttpClientFactory httpClient, ILogger<BookApiService> logger)
        {
            _iHttp = httpClient;
            _logger = logger;
        }
        public string ApiUrl { get; set; } = "https://localhost:7262";

        //GetAllBooks
        public async Task<List<Book>> GetAllBooks()
        {
            string endpoint = ApiUrl + "/books";
            var client = _iHttp.CreateClient();

            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {

                var jsonApiResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonApiResponse);

                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    var books = apiResponse.Result as List<Book>;

                    if (books != null)
                    {
                        return books; 
                    }
                }

                return new List<Book>();

            }
            else { return new List<Book>(); }
        }
        //GetGenres
        public async Task<List<string>> GetGenres()
        {
            string endpoint = ApiUrl + "/genres";
            var client = _iHttp.CreateClient();

            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var genres = JsonConvert.DeserializeObject<List<string>>(json);
                if (genres != null)
                {
                    return genres;
                }
                else return new List<string>();
            }
            else { return new List<string>(); }
        }
        //DeleteBook
        public async Task<Book> DeleteBook(int id)
        {
            string endpoint = ApiUrl + $"/book/{id}";
            var client = _iHttp.CreateClient();

            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var jsonBook = await response.Content.ReadAsStringAsync();
                var book = JsonConvert.DeserializeObject<Book>(jsonBook);

                if (book != null) 
                {
                    await client.DeleteAsync(endpoint);
                    return book;
                }
            }
            _logger.LogError("DeleteBook from ApiService returned null. Id was not found.");
            return null;
        }
        //UpdateBook

        //GetBy{string} *Search*



        //AddBook
        public async Task<HttpResponseMessage> AddBook(Book bookToAdd)
        {
            string endpoint = "/book";
            var client = _iHttp.CreateClient();
            try
            {
                var response = await client.PostAsJsonAsync(ApiUrl + endpoint, bookToAdd);
                _logger.LogInformation("Post has been made to Database.");
                return response;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Post has faild, {ex.Message}");
                throw;
            }
        }
        //GetBookId
        public async Task<Book> GetBook(int id)
        {
            var client = _iHttp.CreateClient();
            var response = await client.GetAsync($"{ApiUrl}/books/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var book = JsonConvert.DeserializeObject<Book>(jsonResponse);
                    _logger.LogInformation($"Attempting to deserialize Book {id}");
                    return book;
            }
            else
            {
                _logger.LogInformation($"Failed to fetch book ID: {id}");
                return null;
            }
        }

    }
}
