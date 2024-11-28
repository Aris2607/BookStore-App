using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace BookStoreApp.Models
{
    public class Transaction : BaseModel
    {
        public int Id { get; set; }
        public string? OrderId { get; set; }
        [AllowNull]
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string? Book { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }

        public List<Bag>? Bags = new List<Bag>();

    }

}
