using Microsoft.Data.SqlClient;

namespace BookStoreApp.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                _logger.LogError("Database timeout occurred: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsync("The database took too long to respond.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled exception: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An internal server error occurred.");
            }
        }
    }
}