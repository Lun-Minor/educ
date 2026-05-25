using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Сервис для работы с отзывами
/// </summary>
namespace educ
{
 
    public class ReviewService
    {
        public List<Reviews> GetReviewsByBookId(int bookId)
        {
            var allReviews = Core.context.Reviews.Where(r => r.BookId == bookId).OrderByDescending(r => r.CreatedAt).ToList();

            int currentUserId = App.CurrentUser?.Id ?? 0;
            bool isAdmin = App.CurrentUser?.Role == 2;

           
            var filteredReviews = allReviews.Where(r =>!r.IsFrozen || isAdmin || r.UserId == currentUserId ).ToList();

            return filteredReviews;
        }
        public void FreezeReview(int reviewId, string reason)
        {
            var review = Core.context.Reviews.FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                review.IsFrozen = true;
                review.FreezeReazon = reason;
                Core.context.SaveChanges();
            }
        }

        public List<Reviews> GetAllReviews()
        {
            return Core.context.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public void AddReview(Reviews review)
        {
            Core.context.Reviews.Add(review);
            Core.context.SaveChanges();
            UpdateBookRating(review.BookId);
        }
        public void UpdateBookRating(int bookId)
        {
            var book = Core.context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return;

            var reviews = Core.context.Reviews.Where(r => r.BookId == bookId && !r.IsFrozen).ToList();

            if (reviews.Any())
            {
                book.RatingCount = reviews.Count;
                book.AverageRating = (decimal)reviews.Average(r => r.Rating);
            }
            else
            {
                book.RatingCount = 0;
                book.AverageRating = 0;
            }

            Core.context.SaveChanges();
        }
        public List<Reviews> GetReviewsByUserId(int userId)
        {
            return Core.context.Reviews.Where(r => r.UserId == userId).OrderByDescending(r => r.CreatedAt).ToList();
        }
    }
}