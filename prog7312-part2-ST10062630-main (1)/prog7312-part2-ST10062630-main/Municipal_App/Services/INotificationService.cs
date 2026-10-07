using Municipal_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Municipal_App.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message);
        Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<Notification> CreateNotificationAsync(int userId, string title, string message, string type, string? actionUrl = null, string? icon = null, int? relatedId = null);
        Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllNotificationsAsReadAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task NotifyIssueStatusChangeAsync(int issueId, string oldStatus, string newStatus);
        Task BroadcastServiceAlertAsync(string title, string message, string? category = null);
    }
}
