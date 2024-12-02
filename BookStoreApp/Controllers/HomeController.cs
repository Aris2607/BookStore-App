using BookStoreApp.Models;
using BookStoreApp.Data.Interfaces;
using BookStoreApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookStoreApp.ViewModels;
using BookStoreApp.Filters;

namespace BookStoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;

        public HomeController(ILogger<HomeController> logger, IBookService bookService)
        {
            (_logger, _bookService) = (logger, bookService);
        }

        [NotForAdmin]
        public async Task<IActionResult> Index(string query, int page = 1, int pageSize = 10)
        {
            try
            {
                List<Book> mainBooks = await _bookService.GetAllBooksAsync(page, pageSize);
                List<Book> searchBooks = new List<Book>();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    searchBooks = await _bookService.SearchBook(query);
                }

                HomeViewModel model = new HomeViewModel
                {
                    MainBooks = mainBooks,
                    SearchBooks = searchBooks,
                    Query = query
                };
                return View(model);
            } catch (Exception x)
            {
                return StatusCode(500);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
