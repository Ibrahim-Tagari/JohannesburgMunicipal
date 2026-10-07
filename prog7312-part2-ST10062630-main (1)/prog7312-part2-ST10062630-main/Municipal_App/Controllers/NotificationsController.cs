using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Municipal_App.Services;
using Municipal_App.Models;
using System.Security.Claims;

namespace Municipal_App.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IPushSubscriptionService _pushSubscriptionService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationService notificationService,
            IPushSubscriptionService pushSubscriptionService,
            ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _pushSubscriptionService = pushSubscriptionService;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var count = await _notificationService.GetUnreadNotificationCountAsync(userId);
            return Ok(new { count });
        }

        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var success = await _notificationService.MarkNotificationAsReadAsync(id, userId);
            return success ? Ok() : NotFound();
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var success = await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return Ok(new { success });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var success = await _notificationService.DeleteNotificationAsync(id, userId);
            return success ? Ok() : NotFound();
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var success = await _pushSubscriptionService.SubscribeUserAsync(
                userId,
                request.Endpoint,
                request.P256dh,
                request.Auth
            );

            return success ? Ok() : BadRequest();
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var success = await _pushSubscriptionService.UnsubscribeUserAsync(userId, request.Endpoint);
            return success ? Ok() : BadRequest();
        }
    }

    public class PushSubscriptionRequest
    {
        public string Endpoint { get; set; } = string.Empty;
        public string P256dh { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
    }

    public class UnsubscribeRequest
    {
        public string Endpoint { get; set; } = string.Empty;
    }
}
