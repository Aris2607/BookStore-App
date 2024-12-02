using BookStoreApp.Data.Interfaces;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.Controllers
{
    [Route("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IBookService bookService, ITransactionService transactionService, ILogger<AdminController> logger)
        {
            _bookService = bookService;
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        [HttpGet("Books")]
        public async Task<IActionResult> Books(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Admin Books triggered");
            try
            {
                var books = await _bookService.GetAllBooksAsync(page, pageSize);
                if (!books.Any())
                {
                    TempData["error"] = "The Book is Empty!";
                    return View();
                }
                return View(books);
            }
            catch (Exception x)
            {
                TempData["error"] = "An Unexpected errors occured";
                return View();
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard() => View();

        [HttpGet("Order")]
        public async Task<IActionResult> Order ()
        {
            try
            {
                var transaction = await _transactionService.GetAllTransaction();

                if (!transaction.Any())
                {
                    TempData["error"] = "There's no transaction";
                    return View();
                }

                return View(transaction);
            } catch (Exception x)
            {
                _logger.LogInformation("An Unexpected Error Occured");
                return StatusCode(500);
            }
        }

        [HttpPost("Deliver/{Id}")]
        public async Task<IActionResult> Deliver(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return BadRequest();
                }

                await _transactionService.UpdateTransactionStatus(Id, "Deliver");
                return Ok();
            } catch (Exception x)
            {
                _logger.LogInformation("Cannot update order status:", x);
                return StatusCode(500);
            }
        }

        [HttpPost("Cancel/{Id}")]
        public async Task<IActionResult> Cancel(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return BadRequest();
                }

                await _transactionService.UpdateTransactionStatus(Id, "Cancelled");
                return Ok();
            }
            catch (Exception x)
            {
                _logger.LogInformation("Cannot update order status:", x);
                return StatusCode(500);
            }
        }

        // Books CRUD

        // GET: /admin/books/create
        [HttpGet("/books/create")]
        public IActionResult Create() => View();

        // POST: /admin/books/create
        [HttpPost("/books/create")]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid) return View(book);

            try
            {
                await _bookService.CreateBookAsync(book);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book.");
                return View("Error");
            }
        }

        // GET: /admin/books/edit/{id}
        [HttpGet("books/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetOneBookAsync(id);
            return book == null ? NotFound() : View(book);
        }

        // POST: /admin/books/edit/{id}
        [HttpPost("books/edit/{id}")]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return BadRequest();

            //if (!ModelState.IsValid) return View(book);

            try
            {
                await _bookService.EditBookAsync(id, book);
                TempData["alert"] = "Book Changed Successfully";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing book.");
                return View("Error");
            }
        }

        [HttpGet("/books/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return BadRequest();
            try
            {
                await _bookService.DeleteBookAsync(id);
                TempData["alert"] = "Book Successfully deleted!";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book.");
                return View("Error");
            }
        }
    }
}
