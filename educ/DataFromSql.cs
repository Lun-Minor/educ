using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace educ
{
    public class DataFromSql
    {
        //книги
        public List<Books> GetAllBooks()
        {
            return Core.context.Books.OrderBy(b => b.Title).ToList();
        }
        public Books GetBookId(int id)
        {
            return Core.context.Books.FirstOrDefault(b => b.Id == id);
        }

        public List<Books> SearchBooks(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) { return GetAllBooks(); }

            return Core.context.Books.Where(b => b.Title.Contains(searchText)).OrderBy(b => b.Title).ToList();
        }

        public List <AuthorRequests> GetAllAuthorRequest()
        {
            return Core.context.AuthorRequests.OrderBy(a=>a.UserId).ToList();
        }
        //жагры
        public List<Genres> GetAllGenres() { 
        return Core.context.Genres.OrderBy(g=>g.Name).ToList();
        }

        //пользователи
        public List <Users> GetAllUsers()
        {
            return Core.context.Users.OrderBy(o=>o.Id).ToList();
        }
        public Users GetUserId(int id)
        {
            return Core.context.Users.FirstOrDefault(u => u.Id == id);
        }

        public List<ReadingList> GetAllReadingList()
        {
            return Core.context.ReadingList.OrderBy(r =>r.BookId).ToList();
        }
        public List<Complaints> GetAllComplaints() { 
        return Core.context.Complaints.OrderBy(o=>o.UserId).ToList();
        }
        public List<UnfreezeRequests> GetAllUnfreezeRequests()
        {
            return Core.context.UnfreezeRequests.OrderBy(o=>o.UserId).ToList();
        }
        public List<Reviews> GetAllReviews() { 
        return Core.context.Reviews.OrderByDescending(r => r.UserId).ToList();
        }

        public void AddBook(Books book)
        {
            Core.context.Books.Add(book);
            Core.context.SaveChanges();
        }

        public void AddReview(Reviews review)
        {
            Core.context.Reviews.Add(review);
            Core.context.SaveChanges();
        }

        public void AddReadingList(ReadingList item)
        {
            var existing = Core.context.ReadingList.FirstOrDefault(r =>r.UserId == item.UserId && r.BookId == item.BookId);

            if (existing != null) { Core.context.ReadingList.Remove(existing); }     

            Core.context.ReadingList.Add(item);
            Core.context.SaveChanges();
        }

        public List<Books> SearchAuthor(string searchText) {
            if (string.IsNullOrWhiteSpace(searchText)) { return GetAllBooks(); }
            return Core.context.Books.Where(b => b.Users.Name.Contains(searchText)).ToList();
        }

        public List<Books> SortingRating() {
            return Core.context.Books.OrderByDescending(b => b.AverageRating).ToList();
        }

      

    }
}
