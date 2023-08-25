using BookLibraryApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace BookLibraryApp.Services
{
    public class APIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public APIService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public string ApiUrl { get; set; } = "https://localhost:7262";
        public Task<HttpResponseMessage> AddBook(Book bookToAdd)
        {
            string endpoint = "/book";
            try
            {
                var response = _httpClient.PostAsJsonAsync(ApiUrl + endpoint, bookToAdd);
                _logger.LogInformation("Post has been made to Database.");
                return response;


            }
            catch (Exception ex)
            {
                _logger.LogError("Post has faild.");
                throw;
            }
        }
    }
}
