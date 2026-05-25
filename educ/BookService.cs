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
        /// <summary>
        /// все книги
        /// </summary>
        
        public List<Books> GetAllBooks()
        {
            return Core.context.Books.OrderBy(b => b.Title).ToList();
        }
        /// <summary>
        /// поиск книги
        /// </summary>
        public List<Books> SearchBooks(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllBooksForCurrentUser();

            string lowerSearch = searchText.Trim().ToLower();

            var allBooks = Core.context.Books.ToList();

            return allBooks
                .Where(b =>
                    b.Users != null &&
                    b.Users.IsFrozen == false &&
                    (b.Title.ToLower().Contains(lowerSearch) ||
                     b.Users.Name.ToLower().Contains(lowerSearch))
                ).OrderByDescending(b => b.AverageRating).ToList();
        }
        /// <summary>
        /// поиск книг по жанрам
        /// </summary>
        public List<Books> GetBooksByGenre(int genreId)
        {
            if (genreId <= 0) { return GetAllBooks(); }
            return Core.context.Books.Where(b => b.Genres.Any(g => g.Id == genreId)).OrderByDescending(b => b.AverageRating).ToList();
        }
        /// <summary>
        /// сортировка книг по рейтингу
        /// </summary>
        public List<Books> GetBooksSortedByRating(bool descending = true)
        {
            if (descending)
                return Core.context.Books.OrderByDescending(b => b.AverageRating).ToList();
            else
                return Core.context.Books.OrderBy(b => b.AverageRating).ToList();
        }
        /// <summary>
        /// сортировка книг по названию
        /// </summary>
        public List<Books> GetBooksSortedByName(bool descending = false)
        {
            if (descending)
                return Core.context.Books.OrderByDescending(b => b.Title).ToList();
            else
                return Core.context.Books.OrderBy(b => b.Title).ToList();
        }
        public Books GetBookById(int id)
        {
            return Core.context.Books.FirstOrDefault(b => b.Id == id);
        }
        /// <summary>
        /// книги по автору
        /// </summary>
        public List<Books> GetBooksByAuthorId(int authorId)
        {
            return Core.context.Books.Where(b => b.AuthorId == authorId).OrderByDescending(b => b.CreatedDate).ToList();
        }
        /// <summary>
        /// получение книг по жанрам
        /// </summary>
        public List<Genres> GetAllGenres()
        {
            return Core.context.Genres.OrderBy(g => g.Name).ToList();
        }
        //добавление книги
        public void AddBook(Books book)
        {
            Core.context.Books.Add(book);
            Core.context.SaveChanges();
        }
        /// <summary>
        /// книги для конкретного пользователя
        /// </summary>
        public List<Books> GetAllBooksForCurrentUser()
        {
            if (App.CurrentUser == null) return new List<Books>();

            var allBooks = Core.context.Books.ToList();

            var visibleBooks = allBooks.Where(b =>
                b.Users != null &&
                b.Users.IsFrozen == false
            ).ToList();

            return visibleBooks.OrderBy(b => b.Title).ToList();
        }
        /// <summary>
        /// замороженная книга
        /// </summary>
        public void FreezeBook(int bookId, string reason)
        {
            var book = Core.context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                book.IsFrozen = true;
                book.FreezeReazon = reason;
                Core.context.SaveChanges();
            }
        }
        /// <summary>
        /// подсчет рейтинга
        /// </summary>
        public void RecalculateBookRating(int bookId)
        {
            var reviewService = new ReviewService();
            reviewService.UpdateBookRating(bookId);
        }
    }
}