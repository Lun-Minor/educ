using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace educ
{
    public class DataFromSql
    {
        public List<Books> GetAllBooks()
        {
            return Core.context.Books
                               .OrderBy(b => b.Title)
                               .ToList();
        }
    }
}
