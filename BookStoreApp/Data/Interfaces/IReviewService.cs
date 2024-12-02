using BookStoreApp.Models;

namespace BookStoreApp.Data.Interfaces
{
    public interface IReviewService
    {
        Task<List<Review>> GetReviews();
        Task AddReview(Review review);
    }
}
