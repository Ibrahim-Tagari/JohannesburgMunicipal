using Municipal_App.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Municipal_App.Services
{
    public interface ILocalEventService
    {
        Task<List<LocalEvent>> GetAllEventsAsync();
        Task<LocalEvent?> GetEventByIdAsync(int id);
        Task<List<LocalEvent>> SearchEventsAsync(string? searchQuery, string? category, DateTime? startDate, DateTime? endDate);
        Task<List<string>> GetAllCategoriesAsync();
        Task<Dictionary<string, int>> GetEventCountByCategoryAsync();
        Task<List<LocalEvent>> GetUpcomingEventsAsync(int count = 10);
        Task IncrementViewCountAsync(int eventId);
    }
}
