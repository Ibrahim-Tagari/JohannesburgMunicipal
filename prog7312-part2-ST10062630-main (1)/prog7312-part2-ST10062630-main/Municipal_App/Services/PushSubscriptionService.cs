using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Municipal_App.Data;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    public class PushSubscriptionService : IPushSubscriptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PushSubscriptionService> _logger;

        public Task SaveSubscriptionAsync(string endpoint, string p256dh, string auth)
        {
            Console.WriteLine($"Subscription saved for endpoint: {endpoint}");
            return Task.CompletedTask;
        }

        public PushSubscriptionService(ApplicationDbContext context, ILogger<PushSubscriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> SubscribeAsync(PushSubscription subscription)
        {
            try
            {
                var existing = await _context.PushSubscriptions
                    .FirstOrDefaultAsync(ps => ps.UserId == subscription.UserId && ps.Endpoint == subscription.Endpoint);

                if (existing != null)
                {
                    existing.IsActive = true;
                    existing.P256dh = subscription.P256dh;
                    existing.Auth = subscription.Auth;
                }
                else
                {
                    _context.PushSubscriptions.Add(subscription);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to push notifications");
                return false;
            }
        }

        public async Task<bool> UnsubscribeAsync(int userId, string endpoint)
        {
            try
            {
                var subscription = await _context.PushSubscriptions
                    .FirstOrDefaultAsync(ps => ps.UserId == userId && ps.Endpoint == endpoint);

                if (subscription == null) return false;

                subscription.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from push notifications");
                return false;
            }
        }

        public async Task<bool> SubscribeUserAsync(int userId, string endpoint, string p256dh, string auth)
        {
            var subscription = new PushSubscription
            {
                UserId = userId,
                Endpoint = endpoint,
                P256dh = p256dh,
                Auth = auth,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            return await SubscribeAsync(subscription);
        }

        public async Task<bool> UnsubscribeUserAsync(int userId, string endpoint)
        {
            return await UnsubscribeAsync(userId, endpoint);
        }
    }
}
