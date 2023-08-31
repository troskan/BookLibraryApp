using Azure;
using BookLibraryApi.Models;
using BookLibraryApi.Repositories.Interface;
using BookLibraryApp.Models;
using BookLibraryApp.Services;
using BookLibraryApp.Services.IServices;
using BookLibraryApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace BookLibraryApp.Controllers
{
    public class BookController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookController> _logger;
        private readonly BookApiService _apiService;
        private readonly IBookService _bookService;

        public BookController(IBookService bookService, HttpClient httpClient, ILogger<BookController> logger, BookApiService apiService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiService = apiService;

            _bookService = bookService;
        }

        //Index - Get All Books
        public async Task<IActionResult> Index()
        {
            List<Book> list = new();

            var response = await _bookService.GetAllAsync<ApiResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        //Create - Get Create View
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<string> list = new();

            try
            {

                var response = await _bookService.GetGenres<ApiResponse>();

                if (response != null && response.IsSuccess)
                {
                    list = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(response.Result));

                }

                list.Add("No listings");
                return View(list);
            }
            catch (Exception)
            {
                return View(list);
            }
        }

        //Create - Post Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {

                var response = await _bookService.CreateAsync<ApiResponse>(book, "");
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Book created successfully";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Error encountered.";
            return RedirectToAction("Index");
        }

        //Put - Get Put View
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Edit GET called with id: {id}");

            var genreResponse = await _bookService.GetGenres<ApiResponse>();
            var bookResponse = await _bookService.GetAsync<ApiResponse>(id, "");

            _logger.LogInformation($"Genre Response: {Convert.ToString(genreResponse.Result)}");
            _logger.LogInformation($"Book Response: {Convert.ToString(bookResponse.Result)}");

            var genreList = new List<string>();

            if (genreResponse.Result != null && !string.IsNullOrEmpty(genreResponse.Result.ToString()))
            {
                try
                {
                    genreList = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(genreResponse.Result));
                }
                catch (JsonReaderException ex)
                {
                    _logger.LogError($"JsonReaderException while deserializing Genre Response: {ex.Message}");
                }

            }
            else
            {
                _logger.LogWarning("Genre Response is null or empty.");
            }

            Book book = JsonConvert.DeserializeObject<Book>(Convert.ToString(bookResponse.Result));

            var viewModel = new BookEditViewModel()
            {
                Book = book,
                Genres = genreList
            };

            return View(viewModel);
        }
        //Put - Put Request
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] Book book)
        {
            _logger.LogInformation($"Edit PUT called with book: {JsonConvert.SerializeObject(book)}");

            if (ModelState.IsValid)
            {
                // Call your service to update the book
                var response = await _bookService.UpdateAsync<ApiResponse>(book, "");

                if (response.IsSuccess)
                {
                    _logger.LogInformation("Book updated successfully.");
                    return Ok(new { Message = "Book updated successfully" });
                }
                else
                {
                    _logger.LogWarning("Failed to update the book.");
                    return BadRequest(new { Message = "Failed to update the book" });
                }
            }

            _logger.LogWarning("Invalid model.");
            return BadRequest(new { Message = "Invalid model" });
        }
        //Delete - Remove Book
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _bookService.DeleteAsync<ApiResponse>(id, "");
                _logger.LogInformation($"Attempting to delete id {id}");

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError("Exception from deleting ID.");
                return RedirectToAction("Error");
            }
        }
        //Details - Get Book Id
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _bookService.GetAsync<ApiResponse>(id, "");
                if (response != null)
                {
                    _logger.LogInformation($"Details object content: {response.Result}");
                    var book = JsonConvert.DeserializeObject<Book>(Convert.ToString(response.Result));
                    return View(book);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> Search(string searchString)
        {
            var list = new List<Book>();
            var response = await _bookService.Search<ApiResponse>(searchString);
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
                return View("Index", list);
            }
            return View("Error");
        }

    }

    ////GetAllGenres
    //[HttpGet]
    //public async Task<ActionResult> Create()
    //{
    //    try
    //    {
    //        var genres = await _apiService.GetGenres();
    //        return View(genres);
    //    }
    //    catch (Exception)
    //    {
    //        return View("Error");
    //        throw;
    //    }

    //}

    ////PostBook
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> Create(Book book)
    //{
    //    try
    //    {
    //        await _apiService.AddBook(book);
    //        return View(book);
    //    }
    //    catch (Exception)
    //    {
    //        return RedirectToAction("Index");
    //    }
    //}
    ////GetUpdateBook
    //[HttpGet]
    //public async Task<ActionResult> Edit(int id)
    //{
    //    var bookApi = $"https://localhost:7262/books/{id}";
    //    var genresUrl = "https://localhost:7262/genres";

    //    var bookResponse = await _httpClient.GetAsync(bookApi);
    //    _logger.LogInformation($"Response from API: {await bookResponse.Content.ReadAsStringAsync()}");

    //    if (!bookResponse.IsSuccessStatusCode)
    //    {
    //        _logger.LogError("Failed to retrieve book object.");
    //        return View("Error");
    //    }
    //    var bookToEdit = await bookResponse.Content.ReadFromJsonAsync<Book>();

    //    var genreResponse = await _httpClient.GetAsync(genresUrl);
    //    if (!genreResponse.IsSuccessStatusCode)
    //    {
    //        _logger.LogError("Failed to retrieve genre object.");
    //        return View("Error");
    //    }
    //    var genres = await genreResponse.Content.ReadFromJsonAsync<List<string>>();

    //    var viewModel = new BookEditViewModel
    //    {
    //        Book = bookToEdit,
    //        Genres = genres
    //    };

    //    return View(viewModel);
    //}
    ////UpdateBook
    //[HttpPost]
    //public async Task<ActionResult> Edit(int id, Book bookToEdit) 
    //{
    //    var apiUrl = $"https://localhost:7262/book/{id}";
    //    var genresUrl = "https://localhost:7262/genres";

    //    var bookJson = JsonConvert.SerializeObject(bookToEdit);
    //    var content = new StringContent(bookJson, Encoding.UTF8, "application/json");

    //    var response = await _httpClient.PutAsync(apiUrl, content);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        _logger.LogInformation("API PUT SUCCESSFUL, RETURN TO INDEX");
    //        return RedirectToAction("Index");
    //    }
    //    else
    //    {
    //        _logger.LogError($"Could not post Model. {response.Content.ReadAsStringAsync()}");
    //        var genresResponse = await _httpClient.GetAsync(genresUrl);
    //        if (!genresResponse.IsSuccessStatusCode)
    //        {
    //            var responseBody = await genresResponse.Content.ReadAsStringAsync();
    //            _logger.LogInformation($"Genres Response: {responseBody}");
    //            return View("Error");
    //        }
    //        var genres = await genresResponse.Content.ReadFromJsonAsync<List<string>>();



    //        var viewModel = new BookEditViewModel
    //        {
    //            Book = bookToEdit,
    //            Genres = genres
    //        };

    //        // Log the error or display it to the user
    //        var errorMessage = await response.Content.ReadAsStringAsync();
    //        ModelState.AddModelError(string.Empty, errorMessage);
    //        return View(viewModel);  // Return to the edit view with the model to show errors
    //    }

    //}



    ////GetBook{string} *Search*
    //public async Task<IActionResult> Search(string searchString)
    //{
    //    var response = await _httpClient.GetAsync($"https://localhost:7262/search?searchString={searchString}");

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var jsonResponse = await response.Content.ReadAsStringAsync();
    //        var books = JsonConvert.DeserializeObject<List<Book>>(jsonResponse);
    //        return View("Index", books);
    //    }
    //    _logger.LogError("Error could not fetch Search result");
    //    return View("Error");
    //}


}
