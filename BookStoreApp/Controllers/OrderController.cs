using BookStoreApp.Data;
using BookStoreApp.Data.Interfaces;
using BookStoreApp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    [Route("order")]
    public class OrderController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly BookStoreAppContext _context;

        public OrderController(ITransactionService transactionService, BookStoreAppContext context)
        {
            _transactionService = transactionService;
            _context = context;
        }

        [AllowAnonymous]
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Order()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }
            var orderList = await _transactionService.GetAllTransactionByUserId(userId);

            return View(orderList);
        }

        [NotFoundIfNotAllowed]
        //[Authorize(Roles = "User")]
        [HttpGet("Thankyou")]
        public async Task<IActionResult> ThankYou([FromQuery] string Order) {
            
            var order = await _context.Transactions.FirstOrDefaultAsync(t => t.OrderId == Order);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
