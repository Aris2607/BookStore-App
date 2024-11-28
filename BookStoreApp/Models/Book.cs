namespace BookStoreApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PublicationYear { get; set; }
        public int CategoryId { get; set; }
        public int PageCount { get; set; }
        public string Summary { get; set; }
        public string CoverImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? ReviewsCount { get; set; }
        public decimal Price { get; set; }
        public DateOnly? IsDeleted { get; set; }

        public Category? Category { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }

}
