using Microsoft.AspNetCore.Identity;

namespace BookStoreApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
