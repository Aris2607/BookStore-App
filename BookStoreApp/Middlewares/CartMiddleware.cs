using BookStoreApp.Models;
using System.Text.Json;
using System.Text;
using BookStoreApp.Data;


namespace BookStoreApp.Middlewares
{
    public class CartMiddleware
    {
        private readonly RequestDelegate _next;

        public CartMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var cartCount = 0;
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BookStoreAppContext>();
                cartCount = dbContext.Bags.Where(b => b.UserId == userId).ToList().Count;
            }
            //var currentCart = context.Session.GetString("Cart") ?? JsonSerializer.Serialize(new List<TransactionItem>());
            //context.Session.SetString("Cart", currentCart);

            //var cartCount = JsonSerializer.Deserialize<List<TransactionItem>>(currentCart).Count();

            context.Items["CartCount"] = cartCount;

            // Continue processing the request
            await _next(context);
        }
        //public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        //{
        //    // Resolve DbContext from the service provider
        //    using (var scope = serviceProvider.CreateScope())
        //    {
        //        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            }
        }