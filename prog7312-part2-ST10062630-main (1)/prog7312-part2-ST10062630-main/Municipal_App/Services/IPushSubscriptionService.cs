using System.Threading.Tasks;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    public interface IPushSubscriptionService
    {
        Task SaveSubscriptionAsync(string endpoint, string p256dh, string auth);
        Task<bool> SubscribeAsync(PushSubscription subscription);
        Task<bool> UnsubscribeAsync(int userId, string endpoint);
        Task<bool> SubscribeUserAsync(int userId, string endpoint, string p256dh, string auth);
        Task<bool> UnsubscribeUserAsync(int userId, string endpoint);
    }
}
