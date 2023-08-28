using BookLibraryApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace BookLibraryApp.Services
{
    public class APIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public APIService(HttpClient httpClient, ILogger<APIService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public string ApiUrl { get; set; } = "https://localhost:7262";

        //GetAllBooks
        public async Task<List<Book>> GetAllBooks()
        {
            string endpoint = ApiUrl + "/books";

            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {

                var jsonBook = await response.Content.ReadAsStringAsync();
                var book = JsonConvert.DeserializeObject<List<Book>>(jsonBook);

                if (book != null)
                {
                    return book;
                }

                return new List<Book>();

            }
            else { return new List<Book>(); }
        }
        //DeleteBook

        //UpdateBook

        //GetBy{string} *Search*



        //AddBook
        public async Task<HttpResponseMessage> AddBook(Book bookToAdd)
        {
            string endpoint = "/book";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiUrl + endpoint, bookToAdd);
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
            var response = await _httpClient.GetAsync($"{ApiUrl}/books/{id}");

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

                throw new Exception($"Failed to fetch book with id: /books/{id}");
            }
        }

    }
}
