using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStoreApp.Data.Interfaces;
using BookStoreApp.Models;
using BookStoreApp.Services;
using BookStoreApp.Controllers.Model;
using Nest;
using BookStoreApp.Filters;

namespace BookStoreApp.Controllers
{
    [Route("books")] // Base route for user-facing pages
    [NotForAdmin]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;
        private readonly ITransactionService _transactionService;
        private readonly IErrorService _errorService;
        //private readonly IElasticClient _elasticClient;

        public BooksController(IBookService bookService, ILogger<BooksController> logger, ITransactionService transactionService, IErrorService errorService)
        {
            _bookService = bookService;
            _logger = logger;
            _transactionService = transactionService;
            _errorService = errorService;
            //_elasticClient = elasticClient;
        }

        // ----- User Routes -----

        // GET: /books
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? null;
            try
            {
                var books = await _bookService.GetAllBooksAsyncP(page, pageSize);
                if (!books.Books.Any())
                {
                    TempData["info"] = "The books still empty! Come again later!";
                    return View(new Book());
                }
                var totalBooks = books.TotalBooks;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
                return View(books.Books);
            }
            catch (Exception x)
            {
                _logger.LogError(x, "Error retrieving books.");
                await _errorService.Error(500, x.Message, x.StackTrace);
                return StatusCode(500);
            }
        }

        public IActionResult Search() => View();

        // Elasticsearch 
        //[HttpPost]
        //public async Task<IActionResult> Search(string query)
        //{
        //    if (string.IsNullOrEmpty(query))
        //    {
        //        return View("Index", new List<Book>());
        //    }

        //    // Search in Elasticsearch
        //    var searchResponse = await _elasticClient.SearchAsync<Book>(s => s
        //        .Query(q => q
        //            .MultiMatch(m => m
        //                .Fields(f => f
        //                    .Field(b => b.Title)
        //                    .Field(b => b.Author)
        //                    .Field(b => b.Summary))
        //                .Query(query)
        //            )
        //        )
        //    );

        //    // Pass results to the view
        //    var books = searchResponse.Documents.ToList();
        //    return View("Index", books);
        //}

        // GET: /books/product/{id}
        [HttpGet("Product/{id}")]
        public async Task<IActionResult> Product(int id)
        {
            try
            {
                var book = await _bookService.GetOneBookAsync(id);
                return book == null ? NotFound() : View(book);
            } catch(Exception x)
            {
                await _errorService.Error(500, x.Message, x.StackTrace);
                return StatusCode(500);
            }
        }

        [Authorize]
        // POST: /books/product
        [HttpPost("AddToBag")]
        public async Task<IActionResult> AddToBag(Book book)
        {
            //var transactionItem = new TransactionItem
            //{
            //    BookId = book.Id,
            //    Title = book.Title,
            //    Price = book.Price,
            //    Image = book.CoverImageUrl,
            //    Qty = 1
            //};

            //_transactionService.AddItem(transactionItem);
            //TempData["alert"] = "The book was successfully added to the cart.";

            //return RedirectToAction(nameof(Product), new { id = book.Id });
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var bag = new Bag
                {
                    UserId = userId,
                    BookId = book.Id,
                    Quantity = 1
                };
                TempData["alert"] = "The book was added";

                await _transactionService.AddBagItem(bag);
                return RedirectToAction(nameof(Product), new { id = book.Id });
            }
            catch (Exception x)
            {
                TempData["error"] = "Cannot add the book, Please to try again later";
                await _errorService.Error(500, x.Message, x.StackTrace);
                return RedirectToAction(nameof(Product), new { id = book.Id });
            }

        }

        // GET: /books/bag
        [Authorize]
        [HttpGet("bag")]
        public async Task<IActionResult> Bag()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var items = await _transactionService.GetAllBagAsync(userId);
                if (!items.Any())
                {
                    throw new NullReferenceException("The bag is empty!");
                }

                decimal subtotal = items.Sum(item => item.Book.Price * (decimal)item.Quantity);
                var tax = subtotal * 0.11m;
                var total = subtotal + tax;

                var bagViewModel = new BagViewModel
                {
                    Bags = items,
                    Subtotal = subtotal,
                    Tax = tax,
                    Total = total
                };
                return View(bagViewModel);
            }catch (NullReferenceException x)
            {
                TempData["error"] = "Your Bag Is Still Empty! Let's get some book!";
                return View(new BagViewModel());
            }catch (Exception x)
            {
                await _errorService.Error(500, x.Message, x.StackTrace);
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost("direct-to-payment")]
        public IActionResult DirectToPayment(BagViewModel bagViewModel)
        {
            HttpContext.Session.SetString("subtotal", bagViewModel.Subtotal.ToString());
            HttpContext.Session.SetString("total", bagViewModel.Total.ToString());
            HttpContext.Session.SetString("tax", bagViewModel.Tax.ToString());

            return RedirectToAction(nameof(Payment));
        }

        [Authorize]
        // GET: /books/payment
        [HttpGet("payment")]
        public async Task<IActionResult> Payment(int? id)
        {
            var book = id != null
                ? await _bookService.GetOneBookAsync(id)
                : new Book { Price = 0, Title = "" };

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "You must be logged in to proceed.";
                return RedirectToAction("Login", "Account");
            }

            if (id != null && book == null)
            {
                TempData["error"] = "The selected book does not exist.";
                return RedirectToAction("Index");
            }


            try
            {
                var items = await _transactionService.GetAllBagAsync(userId);
                if (!items.Any())
                {
                    throw new NullReferenceException("The bag is empty! let's get some books!");
                }

                decimal.TryParse(HttpContext.Session.GetString("subtotal"), out var subtotal);
                decimal.TryParse(HttpContext.Session.GetString("total"), out var total);
                decimal.TryParse(HttpContext.Session.GetString("tax"), out var tax);

                var bagViewModel = new BagViewModel
                {
                    Bags = items,
                    Subtotal = subtotal,
                    Total = total,
                    Tax = tax
                };

                var paymentViewModel = new PaymentViewModel
                {
                    BookId = id ?? Convert.ToInt32(HttpContext.Session.GetString("bookId")),
                    Book = book,
                    ContactResponse = new ContactResponse
                    {
                        Name = HttpContext.Session.GetString("name") ?? "",
                        Mobile = HttpContext.Session.GetString("mobile") ?? "",
                        Address = HttpContext.Session.GetString("address") ?? ""
                    },
                    BagViewModel = bagViewModel
                };

                return View(paymentViewModel);
            }
            catch (NullReferenceException x)
            {
                TempData["Error"] = x.Message;
                return RedirectToAction("bag", "books");
            }
            catch (Exception x)
            {
                await _errorService.Error(500, x.Message, x.StackTrace);
                return StatusCode(500);
            }
        }

        // POST: /books/payment
        [Authorize]
        [HttpPost("payment")]
        public async Task<IActionResult> ApplyDiscount(PaymentViewModel model)
        {
            try
            {
                var book = await _bookService.GetOneBookAsync(model.BookId);
                if (book == null)
                {
                    throw new NullReferenceException("Cannot applying discount, try again later.");
                }
                model.Book = book;
                model.ContactResponse ??= new ContactResponse();

                model.DiscountAmount = model.PromoCode switch
                {
                    "DISCOUNT10" => model.Book.Price * 0.10m,
                    "NAHIDA" => model.Book.Price * 0.90m,
                    _ => 0m
                };

                model.Book.Price -= model.DiscountAmount;
                return View(model);
            } 
            catch (NullReferenceException x)
            {
                TempData["error"] = x.Message;
                return View(model);
            }
            catch (Exception x)
            {
                await _errorService.Error(500, x.Message, x.StackTrace);
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost("confirm-contact")]
        public IActionResult ConfirmContact(PaymentViewModel model)
        {
            HttpContext.Session.SetString("bookId", model.BookId.ToString());
            HttpContext.Session.SetString("name", model.ContactResponse.Name);
            HttpContext.Session.SetString("mobile", model.ContactResponse.Mobile);
            HttpContext.Session.SetString("address", model.ContactResponse.Address);

            return RedirectToAction(nameof(Payment));
        }

        [Authorize]
        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(PaymentViewModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var orderId = Guid.NewGuid().ToString();
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "You must be logged in to continue the payment";
                return RedirectToAction("Login", "Account");
            }

            var transaction = new Transaction
            {
                UserId = userId,
                OrderId = orderId,
                CustomerName = model.ContactResponse.Name,
                Address = model.ContactResponse.Address,
                Total = Convert.ToDecimal(HttpContext.Session.GetString("total")),
                Status = "Pending",
                Bags = await _transactionService.GetAllBagAsync(userId)
            };
            try
            {
                await _transactionService.CreateTransaction(transaction);
                return RedirectToAction("ThankYou", "Order", new {Order = orderId});
            } catch (Exception x)
            {
                TempData["error"] = "Cannot Make a Transaction";
                await _errorService.Error(500, x.Message, x.StackTrace);
                return View(model);
                //return RedirectToAction("HandleError", "Errors");
            }


        }

        // ----- Admin Routes -----

        //[Authorize(Roles = "Admin")]
        //[HttpGet("admin/books/list")]
        //public async Task<IActionResult> Admin()
        //{
        //    _logger.LogInformation("Admin Books triggered");
        //    try
        //    {
        //        var books = await _bookService.GetAllBooksAsync();
        //        if (!books.Any())
        //        {
        //            TempData["error"] = "The Book is Empty!";
        //            return View("~/Views/Admin/Index.cshtml");
        //        }
        //        return View("~/Views/Admin/Index.cshtml", books);
        //    } catch (Exception x)
        //    {
        //        TempData["error"] = "An Unexpected errors occured";
        //        return View("~/Views/Admin/Index.cshtml");
        //    }
        //}

        //// GET: /admin/books/create
        //[Authorize(Roles = "Admin")]
        //[HttpGet("/admin/books/create")]
        //public IActionResult Create() => View();

        //// POST: /admin/books/create
        //[Authorize(Roles = "Admin")]
        //[HttpPost("/admin/books/create")]
        //public async Task<IActionResult> Create(Book book)
        //{
        //    if (!ModelState.IsValid) return View(book);

        //    try
        //    {
        //        await _bookService.CreateBookAsync(book);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating book.");
        //        return View("Error");
        //    }
        //}

        //// GET: /admin/books/edit/{id}
        //[Authorize(Roles = "Admin")]
        //[HttpGet("{id}")]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var book = await _bookService.GetOneBookAsync(id);
        //    return book == null ? NotFound() : View(book);
        //}

        //// POST: /admin/books/edit/{id}
        //[Authorize(Roles = "Admin")]
        //[HttpPost("{id}")]
        //public async Task<IActionResult> Edit(int id, Book book)
        //{
        //    if (id != book.Id) return BadRequest();

        //    //if (!ModelState.IsValid) return View(book);

        //    try
        //    {
        //        await _bookService.EditBookAsync(id, book);
        //        TempData["alert"] = "Book Changed Successfully";
        //        return RedirectToAction("Index", "Admin");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error editing book.");
        //        return View("Error");
        //    }
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpGet("delete/{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    if (id == null) return BadRequest();
        //    try
        //    {
        //        await _bookService.DeleteBookAsync(id);
        //        TempData["alert"] = "Book Successfully deleted!";
        //        return RedirectToAction("Index", "Admin");
        //    } catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting book.");
        //        return View("Error");
        //    }
        //}
    }
}
