using BookStoreApp.Data;
using BookStoreApp.Data.Interfaces;

namespace BookStoreApp.Services
{
    public class ErrorService : IErrorService
    {
        private readonly BookStoreAppContext _context;

        public ErrorService(BookStoreAppContext context)
        {
            _context = context;
        }

        public async Task Error(int code, string message, string stacktrace)
        {
            _context.Errors.Add(new Models.Error { ErrorCode = code, Message = message, StackTrace = stacktrace });
            await _context.SaveChangesAsync();
        }
    }
}
