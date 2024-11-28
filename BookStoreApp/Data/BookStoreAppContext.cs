using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookStoreApp.Data
{
    public class BookStoreAppContext : IdentityDbContext<ApplicationUser>
    {
        public BookStoreAppContext (DbContextOptions<BookStoreAppContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        //public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Bag> Bags { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships and additional configuration
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books) // Updated from c.Book to c.Books
                .HasForeignKey(b => b.CategoryId);

            modelBuilder.Entity<Bag>()
                .HasOne(b => b.User)
                .WithMany() // Assuming a Bag doesn't have a collection on User
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Bag>()
                .HasOne(b => b.Book)
                .WithMany() // Assuming a Book doesn't have a collection on Bag
                .HasForeignKey(b => b.BookId);

            //modelBuilder.Entity<Wishlist>()
            //    .HasOne(b => b.User)
            //    .WithMany()
            //    .HasForeignKey(b => b.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User) // Each transaction has one user
                .WithMany()           // One user can have many transactions (no navigation property on ApplicationUser)
                .HasForeignKey(t => t.UserId) // Foreign Key: UserId in Transaction
                .OnDelete(DeleteBehavior.SetNull);
                //.HasOne(t => t.Book, b =>
                //{
                //    b.ToJson(); // Use this if supported (EF Core 8+)
                //});

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany() // Assuming a Review doesn't have a collection on User
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews) // Ensure this matches the Book class definition
                .HasForeignKey(r => r.BookId);
        }


    }
}
