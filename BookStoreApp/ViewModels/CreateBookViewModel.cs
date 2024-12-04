using BookStoreApp.Models;

namespace BookStoreApp.ViewModels
{
    public class CreateBookViewModel
    {
        public Book? Books { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
