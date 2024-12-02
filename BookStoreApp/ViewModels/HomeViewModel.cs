using BookStoreApp.Models;

namespace BookStoreApp.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Book> MainBooks { get; set; }
        public IEnumerable<Book> SearchBooks { get; set; }
        public string Query { get; set; }
    }
}
