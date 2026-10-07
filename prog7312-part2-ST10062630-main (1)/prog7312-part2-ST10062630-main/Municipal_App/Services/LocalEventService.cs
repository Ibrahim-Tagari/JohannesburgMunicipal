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
    public class LocalEventService : ILocalEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LocalEventService> _logger;

        public LocalEventService(ApplicationDbContext context, ILogger<LocalEventService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<LocalEvent>> GetAllEventsAsync()
        {
            try
            {
                return await _context.LocalEvents
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all events");
                return new List<LocalEvent>();
            }
        }

        public async Task<LocalEvent?> GetEventByIdAsync(int id)
        {
            try
            {
                return await _context.LocalEvents.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event {EventId}", id);
                return null;
            }
        }

        public async Task<List<LocalEvent>> SearchEventsAsync(string? searchQuery, string? category, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.LocalEvents.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(e =>
                        e.Title.Contains(searchQuery) ||
                        e.Description.Contains(searchQuery));
                }

                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(e => e.Category == category);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(e => e.EventDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(e => e.EventDate <= endDate.Value);
                }

                return await query.OrderBy(e => e.EventDate).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching events");
                return new List<LocalEvent>();
            }
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            try
            {
                return await _context.LocalEvents
                    .Select(e => e.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return new List<string>();
            }
        }

        public async Task<Dictionary<string, int>> GetEventCountByCategoryAsync()
        {
            try
            {
                return await _context.LocalEvents
                    .GroupBy(e => e.Category)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event counts by category");
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<LocalEvent>> GetUpcomingEventsAsync(int count = 10)
        {
            try
            {
                var now = DateTime.Now;
                return await _context.LocalEvents
                    .Where(e => e.EventDate >= now)
                    .OrderBy(e => e.EventDate)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upcoming events");
                return new List<LocalEvent>();
            }
        }

        public async Task IncrementViewCountAsync(int eventId)
        {
            try
            {
                var eventItem = await _context.LocalEvents.FindAsync(eventId);
                if (eventItem != null)
                {
                    eventItem.ViewCount++;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for event {EventId}", eventId);
            }
        }
    }
}
