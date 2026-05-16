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
            return Core.context.ReadingList.Where(r => r.UserId == userId).ToList();
        }

        public void AddToReadingList(ReadingList item)
        {
            var existing = Core.context.ReadingList.FirstOrDefault(r => r.UserId == item.UserId && r.BookId == item.BookId);

            if (existing != null) { Core.context.ReadingList.Remove(existing);}
            Core.context.ReadingList.Add(item);
            Core.context.SaveChanges();
        }
    }
}