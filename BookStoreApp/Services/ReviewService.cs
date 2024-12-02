using BookStoreApp.Data;
using BookStoreApp.Data.Interfaces;
using BookStoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly BookStoreAppContext _context;

        public ReviewService(BookStoreAppContext context)
        {
            _context = context;
        }
        public async Task<List<Review>> GetReviews() => await _context.Reviews.ToListAsync();

        public async Task AddReview(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

    }
}
