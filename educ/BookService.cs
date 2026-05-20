using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Сервис для работы с книгами
/// </summary>
namespace educ
{
    public class BookService
    {
        public List<Books> GetAllBooks()
        {
            return Core.context.Books.OrderBy(b => b.Title).ToList();
        }

        public List<Books> SearchBooks(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllBooks();

            string lowerSearch = searchText.Trim().ToLower();

            return Core.context.Books.Where(b =>b.Title.ToLower().Contains(lowerSearch) ||
                    (b.Users != null && b.Users.Name.ToLower().Contains(lowerSearch))).OrderByDescending(b => b.AverageRating).ToList();
        }

        public List<Books> GetBooksByGenre(int genreId)
        {
            if (genreId <= 0) { return GetAllBooks(); }
                
            return Core.context.Books.Where(b => b.Genres.Any(g => g.Id == genreId)).OrderByDescending(b => b.AverageRating).ToList();
        }

        public List<Books> GetBooksSortedByRating(bool descending = true)
        {
            if (descending)
            {
                return Core.context.Books.OrderByDescending(b => b.AverageRating).ToList();
            }

            else {
                return Core.context.Books.OrderBy(b => b.AverageRating).ToList();
            }               
        }

        public List<Books> GetBooksSortedByName(bool descending = false)
        {
            if (descending) { return Core.context.Books.OrderByDescending(b => b.Title).ToList(); }

            else { return Core.context.Books.OrderBy(b => b.Title).ToList(); }
                
        }

        public Books GetBookById(int id)
        {
            return Core.context.Books.FirstOrDefault(b => b.Id == id);
        }

        public List<Books> GetBooksByAuthorId(int authorId)
        {
            return Core.context.Books.Where(b => b.AuthorId == authorId).OrderByDescending(b => b.AverageRating).ToList();
        }

        public List<Genres> GetAllGenres()
        {
            return Core.context.Genres.OrderBy(g => g.Name).ToList();
        }

        public void AddBook(Books book)
        {
            Core.context.Books.Add(book);
            Core.context.SaveChanges();
        }
    }
}