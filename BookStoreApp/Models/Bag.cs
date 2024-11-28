using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Bag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public int BookId { get; set; }
        public int? Quantity { get; set; }

        public ApplicationUser User { get; set; }
        public Book Book { get; set; }
    }

}
