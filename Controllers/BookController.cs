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
            _logger.LogCritical("Inside GET Edit");

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

            if (book != null)
            {
                var viewModel = new BookEditViewModel()
                {
                    Book = book,
                    Genres = genreList
                };
                return View(viewModel);
            }
            else
            {
                _logger.LogWarning("Book object is null.");
                return View("Error");
            }
        }

        //Put - Put Request
        [HttpPost]
        public async Task<IActionResult> Edit(Book book)
        {
            _logger.LogInformation($"Edit PUT called with book: {JsonConvert.SerializeObject(book)}");
            _logger.LogCritical("Inside PUT Edit");
            if (ModelState.IsValid)
            {
                var response = await _bookService.UpdateAsync<ApiResponse>(book, "");

                if (response.IsSuccess)
                {
                    _logger.LogInformation("Book updated successfully.");

                    return RedirectToAction("Index");
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

   
}
