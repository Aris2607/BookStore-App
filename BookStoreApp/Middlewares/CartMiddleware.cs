using BookStoreApp.Models;
using System.Text.Json;
using System.Text;


namespace BookStoreApp.Middlewares
{
    public class CartMiddleware
    {
        private readonly RequestDelegate _next;

        public CartMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var currentCart = context.Session.GetString("Cart") ?? JsonSerializer.Serialize(new List<TransactionItem>());
            context.Session.SetString("Cart", currentCart);

            var cartCount = JsonSerializer.Deserialize<List<TransactionItem>>(currentCart).Count();

            context.Items["CartCount"] = cartCount;

            // Continue processing the request
            await _next(context);
        }
    }
}