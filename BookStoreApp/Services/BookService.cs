using BookStoreApp.Data;
using BookStoreApp.Models;
using BookStoreApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Services
{
    public class BookService : IBookService
    {
        private readonly BookStoreAppContext _context;
        private readonly ILogger<BookService> _logger;

        public BookService(BookStoreAppContext context, ILogger<BookService> logger)
        {
            (_context, _logger) = (context, logger);
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            var books = await _context.Books.Where(b => b.IsDeleted == null).ToListAsync();
            return books;
        }

        public async Task<Book> GetOneBookAsync(int? Id) {
            if (Id == null) {
                throw new Exception("Id cannot be null");
            }
            #pragma warning disable CS8603 // Possible null reference return.
            return await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(x => x.Id == Id);
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task CreateBookAsync(Book book) {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task EditBookAsync(int Id, Book book)
        {
            var existingBook = await _context.Books.FindAsync(Id);

            if (existingBook == null)
            {
                throw new KeyNotFoundException("Book not available");
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Summary = book.Summary;
            existingBook.Price = book.Price;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int Id)
        {
            var existingBook = await _context.Books.Where(b => b.IsDeleted == null).FirstOrDefaultAsync(b => b.Id == Id);
            if (existingBook == null)
            {
                throw new KeyNotFoundException("The book not available");
            }

            existingBook.IsDeleted = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();

        }
    }
}
