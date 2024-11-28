namespace BookStoreApp.Middlewares
{
    public class RedirectIfAuthenticatedMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectIfAuthenticatedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                // Redirect user to the home page or dashboard
                context.Response.Redirect("/Home");
                return;
            }

            await _next(context);
        }
    }

}
