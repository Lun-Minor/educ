using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace educ
{
    /// <summary>
    /// Сервис для работы с жанрами
    /// </summary>
    public class GenreService
    {
        public List<Genres> GetAllGenres()
        {
            return Core.context.Genres.OrderBy(g => g.Name).ToList();
        }

        /// <summary>
        /// Обновляет жанры книги 
        /// </summary>
        public void UpdateBookGenres(Books book, List<int> selectedGenreIds)
        {
            if (book == null) return;
            book.Genres.Clear();
            
            var genresToAdd = Core.context.Genres.Where(g => selectedGenreIds.Contains(g.Id)).ToList();

            foreach (var genre in genresToAdd)
            {
                book.Genres.Add(genre);
            }

            Core.context.SaveChanges();
        }
    }
}