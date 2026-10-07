using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Municipal_App.Data;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    public class NotificationService : INotificationService
    {

        public Task SendNotificationAsync(string message)
        {
            Console.WriteLine($"Notification sent: {message}");
            return Task.CompletedTask;
        }

        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
        {
            try
            {
                var query = _context.Notifications.Where(n => n.UserId == userId);

                if (unreadOnly)
                    query = query.Where(n => !n.IsRead);

                return await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user notifications");
                return new List<Notification>();
            }
        }

        public async Task<Notification> CreateNotificationAsync(int userId, string title, string message, string type, string? actionUrl = null, string? icon = null, int? relatedId = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    ActionUrl = actionUrl,
                    Icon = icon,
                    RelatedId = relatedId,
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                throw;
            }
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

                if (notification == null) return false;

                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return false;
            }
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return false;
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            try
            {
                return await _context.Notifications
                    .CountAsync(n => n.UserId == userId && !n.IsRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
                return 0;
            }
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

                if (notification == null) return false;

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                return false;
            }
        }

        public async Task NotifyIssueStatusChangeAsync(int issueId, string oldStatus, string newStatus)
        {
            try
            {
                var issue = await _context.Issues.FindAsync(issueId);
                if (issue == null) return;

                await CreateNotificationAsync(
                    issue.UserId,
                    "Issue Status Updated",
                    $"Your issue #{issueId} status changed from '{oldStatus}' to '{newStatus}'",
                    "status_change",
                    $"/Issues/TrackIssue/{issueId}",
                    "fa-check-circle",
                    issueId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying issue status change");
            }
        }

        public async Task BroadcastServiceAlertAsync(string title, string message, string? category = null)
        {
            try
            {
                var users = await _context.Users.Where(u => u.IsActive).ToListAsync();

                foreach (var user in users)
                {
                    await CreateNotificationAsync(
                        user.Id,
                        title,
                        message,
                        "alert",
                        null,
                        "fa-exclamation-triangle",
                        null
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting service alert");
            }
        }
    }
}
