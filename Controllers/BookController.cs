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
        // GET: BookController
        public async Task<IActionResult> Index(string searchString = null)
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

        // GET: BookController/Details/5
        public async Task<ActionResult> Details(int id)
        {

            var response = await _httpClient.GetAsync($"https://localhost:7262/books/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var book = JsonConvert.DeserializeObject<Book>(jsonResponse);
                return View(book);
            }
            return View("Error");
        }


        //GETGenres
        // POST: BookController/Create
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

        // POST: BookController/Create
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

            //if (ModelState.IsValid)
            //{
            //    var apiUrl = "https://localhost:7262/book";
            //    try
            //    {
            //        var response = await _httpClient.PostAsJsonAsync(apiUrl, book);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            _logger.LogInformation("Post Successful");
            //            return RedirectToAction("Index");

            //        }
            //        else
            //        {
            //            _logger.LogInformation("Post unsuccessful");
            //            return RedirectToAction("Index");

            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex.ToString());
            //        ModelState.AddModelError(string.Empty, "An error occurred while creating the book.");
            //        return View(book);
            //    }

            //}
            //return View(book);

        }
        // GET: book/edit
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
        // POST: BookController/Edit/5
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

        // GET: BookController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var apiDeleteUrl = $"https://localhost:7262/book/{id}";
            var response = await _httpClient.DeleteAsync(apiDeleteUrl);

            return RedirectToAction("Index");
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public async Task<IActionResult> Search(string searchString)
        {
            // Use the query string parameter instead of the route parameter
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
