using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Сервис для работы со списками чтения
/// </summary>


namespace educ
{
    public class ReadingListService
    {
        public List<ReadingList> GetReadingListByUserId(int userId)
        {
            var readingLists = Core.context.ReadingList.Where(r => r.UserId == userId).ToList();

            if (!readingLists.Any()) return readingLists;

            var bookIds = readingLists.Select(r => r.BookId).Distinct().ToList();

            var books = Core.context.Books.Where(b => bookIds.Contains(b.Id)).ToList();

            var authorIds = books.Select(b => b.AuthorId).Distinct().ToList();
            var authors = Core.context.Users.Where(u => authorIds.Contains(u.Id)).ToDictionary(u => u.Id, u => u);

            foreach (var rl in readingLists)
            {
                rl.Books = books.FirstOrDefault(b => b.Id == rl.BookId);
                if (rl.Books != null && authors.ContainsKey(rl.Books.AuthorId))
                {
                    rl.Books.Users = authors[rl.Books.AuthorId];
                }
            }

            return readingLists;
        }

        public void AddToReadingList(ReadingList item)
        {
            var existing = Core.context.ReadingList.FirstOrDefault(r => r.UserId == item.UserId && r.BookId == item.BookId);

            if (existing != null)
            {
                existing.Status = item.Status;
                existing.AddedDate = DateTime.UtcNow;
            }
            else
            {
                Core.context.ReadingList.Add(item);
            }

            Core.context.SaveChanges();
        }

        public void ChangeBookStatus(int userId, int bookId, int newStatus)
        {
            var existing = Core.context.ReadingList.FirstOrDefault(r => r.UserId == userId && r.BookId == bookId);

            if (existing != null)
            {
                existing.Status = newStatus;
                existing.AddedDate = DateTime.UtcNow;
            }
            else
            {
                Core.context.ReadingList.Add(new ReadingList
                {
                    UserId = userId,
                    BookId = bookId,
                    Status = newStatus,
                    AddedDate = DateTime.UtcNow
                });
            }

            Core.context.SaveChanges();
        }

     
        public List<ReadingList> SearchInReadingList(List<ReadingList> list, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return list;

            string lowerSearch = searchText.Trim().ToLower();
            return list.Where(r =>
                (r.Books?.Title != null && r.Books.Title.ToLower().Contains(lowerSearch)) ||
                (r.Books?.Users?.Name != null && r.Books.Users.Name.ToLower().Contains(lowerSearch))).ToList();
        }

        public List<ReadingList> FilterByGenres(List<ReadingList> list, List<int> selectedGenreIds)
        {
            if (selectedGenreIds == null || !selectedGenreIds.Any()) return list;

            return list.Where(r =>r.Books?.Genres != null && r.Books.Genres.Any(g => selectedGenreIds.Contains(g.Id))).ToList();
        }

        public List<ReadingList> SortReadingList(List<ReadingList> list, string sortMode)
        {
            switch (sortMode)
            {
                case "NameAsc":
                    return list.OrderBy(r => r.Books?.Title).ToList();
                case "NameDesc":
                    return list.OrderByDescending(r => r.Books?.Title).ToList();
                case "RatingAsc":
                    return list.OrderBy(r => r.Books?.AverageRating).ToList();
                case "RatingDesc":
                default:
                    return list.OrderByDescending(r => r.Books?.AverageRating).ToList();
            }
        }

      
        public List<ReadingList> GetFilteredReadingList(int userId, int status, string searchText,
            List<int> selectedGenreIds, string sortMode)
        {
            var allUserLists = GetReadingListByUserId(userId);

            var listByStatus = allUserLists.Where(r => r.Status == status).ToList();

            listByStatus = SearchInReadingList(listByStatus, searchText);
            listByStatus = FilterByGenres(listByStatus, selectedGenreIds);
            listByStatus = SortReadingList(listByStatus, sortMode);

            return listByStatus;
        }
    }
}