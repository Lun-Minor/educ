using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace educ
{
 
    public class ReviewService
    {
        public List<Reviews> GetReviewsByBookId(int bookId)
        {
            return Core.context.Reviews.Where(r => r.BookId == bookId).OrderByDescending(r => r.CreatedAt).ToList();
        }

        public List<Reviews> GetAllReviews()
        {
            return Core.context.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public void AddReview(Reviews review)
        {
            Core.context.Reviews.Add(review);
            Core.context.SaveChanges();
        }
    }
}