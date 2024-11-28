using BookStoreApp.Data.Interfaces;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.Controllers
{
    [Route("Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Admin Books triggered");
            try
            {
                var books = await _bookService.GetAllBooksAsync();
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
    }
}
