using BookStoreApp.Models;

namespace BookStoreApp.Data.Interfaces 
{
    public interface IBookService
    {
        public Task<List<Book>> GetAllBooksAsync();
        public Task<Book> GetOneBookAsync(int? Id);
        public Task CreateBookAsync(Book book);
        public Task EditBookAsync(int Id, Book book);
        public Task DeleteBookAsync(int Id);
    }
}