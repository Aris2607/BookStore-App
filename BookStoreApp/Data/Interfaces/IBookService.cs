using BookStoreApp.Models;

namespace BookStoreApp.Data.Interfaces 
{
    public interface IBookService
    {
        public Task<List<Book>> GetAllBooksAsync(int page, int pageSize);
        public Task<BooksItem> GetAllBooksAsyncP(int page, int pageSize);
        public Task<List<Book>> SearchBook(string query);
        public Task<Book> GetOneBookAsync(int? Id);
        public Task CreateBookAsync(Book book);
        public Task EditBookAsync(int Id, Book book);
        public Task DeleteBookAsync(int Id);
    }
}