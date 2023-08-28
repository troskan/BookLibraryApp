using BookLibraryApi.Models;
using BookLibraryApi.Repositories.Interface;
using BookLibraryApp.Services;
using BookLibraryApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using System.Text;

namespace BookLibraryApp.Controllers
{
    public class BookController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookController> _logger;
        private readonly APIService _apiService;
        public BookController(HttpClient httpClient, ILogger<BookController> logger, APIService apiService)
        {
            _logger = logger;
            _httpClient = httpClient;   
            _apiService = apiService;
        }
        // GetAll
        public async Task<IActionResult> Index(string searchString = "")
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                return await Search(searchString);
            }

            var response = await _httpClient.GetAsync("https://localhost:7262/books");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<Book>>(jsonResponse);
                return View(books);
            }
            return View("Error");
        }

        //GetId
        public async Task<ActionResult> Details(int id)
        {

            Book book = await _apiService.GetBook(id);
            return View(book);
        }


        //GetAllGenres
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var apiUrl = "https://localhost:7262/genres";
            var response = await _httpClient.GetAsync(apiUrl);
            List<string> genres = new List<string>();

            if (response.IsSuccessStatusCode)
            {
                genres = await response.Content.ReadFromJsonAsync<List<string>>();
            }
            _logger.LogInformation("Inside Create");
            return View(genres);

        }

        //PostBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Book book)
        {
            try
            {
                await _apiService.AddBook(book);
                return View(book);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        //GetUpdateBook
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var bookApi = $"https://localhost:7262/books/{id}";
            var genresUrl = "https://localhost:7262/genres";

            var bookResponse = await _httpClient.GetAsync(bookApi);
            _logger.LogInformation($"Response from API: {await bookResponse.Content.ReadAsStringAsync()}");

            if (!bookResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve book object.");
                return View("Error");
            }
            var bookToEdit = await bookResponse.Content.ReadFromJsonAsync<Book>();

            var genreResponse = await _httpClient.GetAsync(genresUrl);
            if (!genreResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve genre object.");
                return View("Error");
            }
            var genres = await genreResponse.Content.ReadFromJsonAsync<List<string>>();

            var viewModel = new BookEditViewModel
            {
                Book = bookToEdit,
                Genres = genres
            };

            return View(viewModel);
        }
        //UpdateBook
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Book bookToEdit) 
        {
            var apiUrl = $"https://localhost:7262/book/{id}";
            var genresUrl = "https://localhost:7262/genres";

            var bookJson = JsonConvert.SerializeObject(bookToEdit);
            var content = new StringContent(bookJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("API PUT SUCCESSFUL, RETURN TO INDEX");
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"Could not post Model. {response.Content.ReadAsStringAsync()}");
                var genresResponse = await _httpClient.GetAsync(genresUrl);
                if (!genresResponse.IsSuccessStatusCode)
                {
                    var responseBody = await genresResponse.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Genres Response: {responseBody}");
                    return View("Error");
                }
                var genres = await genresResponse.Content.ReadFromJsonAsync<List<string>>();



                var viewModel = new BookEditViewModel
                {
                    Book = bookToEdit,
                    Genres = genres
                };

                // Log the error or display it to the user
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(viewModel);  // Return to the edit view with the model to show errors
            }

        }

        //DeleteBook
        public async Task<ActionResult> Delete(int id)
        {
            var apiDeleteUrl = $"https://localhost:7262/book/{id}";
            var response = await _httpClient.DeleteAsync(apiDeleteUrl);

            return RedirectToAction("Index");
        }


        //GetBook{string}
        public async Task<IActionResult> Search(string searchString)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7262/search?searchString={searchString}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<Book>>(jsonResponse);
                return View("Index", books);
            }
            _logger.LogError("Error could not fetch Search result");
            return View("Error");
        }

    }
}
