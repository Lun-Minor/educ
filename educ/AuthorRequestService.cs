using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Сервис для работы с заявками на роль автора
/// </summary>

namespace educ
{
    public class AuthorRequestService
    {
        public List<AuthorRequests> GetAllAuthorRequests()
        {
            return Core.context.AuthorRequests.OrderByDescending(a => a.CreatedAt).ToList();
        }

        public List<AuthorRequests> GetPendingAuthorRequests()
        {
            return Core.context.AuthorRequests.Where(a => a.Status == 0).OrderByDescending(a => a.CreatedAt).ToList();
        }

        public void AddAuthorRequest(AuthorRequests request)
        {
            Core.context.AuthorRequests.Add(request);
            Core.context.SaveChanges();
        }

        public void UpdateAuthorRequestStatus(int requestId, int newStatus)
        {
            var request = Core.context.AuthorRequests.FirstOrDefault(a => a.Id == requestId);
            if (request != null)
            {
                request.Status = newStatus;
                Core.context.SaveChanges();
            }
        }
    }
}