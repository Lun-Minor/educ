using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Сервис для работы с жалобами
/// </summary>
namespace educ
{

    public class ComplaintService
    {
        public List<Complaints> GetAllComplaints()
        {
            return Core.context.Complaints.OrderByDescending(c => c.CreatedAt).ToList();
        }

        public List<Complaints> GetPendingComplaints()
        {
            return Core.context.Complaints.Where(c => c.Status == 0).OrderByDescending(c => c.CreatedAt).ToList();
        }

        public void AddComplaint(Complaints complaint)
        {
            Core.context.Complaints.Add(complaint);
            Core.context.SaveChanges();
        }
        
        public void UpdateComplaintStatus(int complaintId, int newStatus, int processedBy)
        {
            var complaint = Core.context.Complaints.FirstOrDefault(c => c.Id == complaintId);
            if (complaint != null)
            {
                complaint.Status = newStatus;
                complaint.ProcessedById = processedBy;
                complaint.ProcessedDate = DateTime.UtcNow;
                Core.context.SaveChanges();
            }
        }
    }
}