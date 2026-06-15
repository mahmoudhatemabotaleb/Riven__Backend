using RivenBackend.Data;
using RivenBackend.Models;
using RivenBackend.Services;

namespace RivenBackend.Services
{
    public interface INotificationService
    {
        Task NotifyHospitalAsync(int hospitalId, int caseId, string type, string message, string? status = "Pending");
    }

    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IRealtimeTrackingService _realtime;

        public NotificationService(AppDbContext context, IRealtimeTrackingService realtime)
        {
            _context = context;
            _realtime = realtime;
        }

        public async Task NotifyHospitalAsync(int hospitalId, int caseId, string type, string message, string? status = "Pending")
        {
            var notification = new Notification
            {
                HospitalId = hospitalId,
                CaseId = caseId,
                Type = type,
                Message = message,
                Status = status ?? "Pending",
                IsRead = false,
                SentTime = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            await _realtime.BroadcastHospitalNotificationAsync(hospitalId, new
            {
                notification.NotificationId,
                notification.HospitalId,
                notification.CaseId,
                notification.Type,
                notification.Message,
                notification.Status,
                notification.IsRead,
                notification.SentTime
            });
        }
    }
}
