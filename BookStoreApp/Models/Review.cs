namespace BookStoreApp.Models
{
    public class Review : BaseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string Content { get; set; }
        public double Rating { get; set; }
        public string Image { get; set; } // Nullable
        public int Likes { get; set; }

        public User User { get; set; }
        public Book Book { get; set; }
    }

}
