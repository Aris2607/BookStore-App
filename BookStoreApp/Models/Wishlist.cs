using Microsoft.AspNetCore.Identity;

namespace BookStoreApp.Models
{
    public class Wishlist : BaseModel
    {
        public int Id { get; set; } // Primary Key
        public string UserId { get; set; } // Foreign Key to AspNetUsers

        // Navigation Property
        public IdentityUser User { get; set; }

        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
    }
}