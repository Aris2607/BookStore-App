using BookStoreApp.Models;
using BookStoreApp.Data.Interfaces;
using BookStoreApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        public async Task<IActionResult> Index()
        {
            List<Book> books = await _bookService.GetAllBooksAsync();
            return View(books);
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
