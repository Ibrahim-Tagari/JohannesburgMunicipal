using System.Collections.Generic;
using System.Threading.Tasks;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    public interface IEventRecommendationService
    {
        Task<List<EventRecommendation>> GetRecommendationsForUserAsync(int userId);
        Task RecordUserSearchAsync(int userId, string searchTerm, string? category, int resultCount);
        Task<List<string>> GetUserInterestsAsync(int userId);
    }
}
