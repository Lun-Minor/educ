using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace educ
{
    public class BookService
    {
        public List<Books> GetAllBooks()
        {
            return Core.context.Books.OrderBy(b => b.Title).ToList();
        }

        public Books GetBookById(int id)
        {
            return Core.context.Books.FirstOrDefault(b => b.Id == id);
        }

        public List<Books> SearchBooks(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) { return GetAllBooks(); }
            

            return Core.context.Books.Where(b => b.Title.Contains(searchText)).OrderBy(b => b.Title).ToList();
        }

        public List<Books> GetBooksByAuthorId(int authorId)
        {
            return Core.context.Books.Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public List<Books> GetBooksSortedByRating()
        {
            return Core.context.Books.OrderByDescending(b => b.AverageRating).ToList();
        }

        public void AddBook(Books book)
        {
            Core.context.Books.Add(book);
            Core.context.SaveChanges();
        }
    }
}