using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace educ
{

    public class UnfreezeRequestService
    {
        public List<UnfreezeRequests> GetAllUnfreezeRequests()
        {
            return Core.context.UnfreezeRequests.OrderByDescending(u => u.CreatedAt).ToList();
        }

        public void AddUnfreezeRequest(UnfreezeRequests request)
        {
            Core.context.UnfreezeRequests.Add(request);
            Core.context.SaveChanges();
        }
    }
}