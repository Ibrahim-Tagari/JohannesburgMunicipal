using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Municipal_App.Data;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    public class EventRecommendationService : IEventRecommendationService
    {
        private readonly ApplicationDbContext _context;

        public EventRecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventRecommendation>> GetRecommendationsForUserAsync(int userId)
        {
            var recommendations = new List<EventRecommendation>();

            var userInterests = await GetUserInterestsAsync(userId);

            var upcomingEvents = await _context.LocalEvents
                .Where(e => e.EventDate > DateTime.Now)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            foreach (var localEvent in upcomingEvents)
            {
                var score = CalculateRelevanceScore(localEvent, userInterests);

                if (score > 0)
                {
                    recommendations.Add(new EventRecommendation
                    {
                        Event = localEvent,
                        RelevanceScore = score,
                        RecommendationReason = GetRecommendationReason(localEvent, userInterests)
                    });
                }
            }

            return recommendations.OrderByDescending(r => r.RelevanceScore).ToList();
        }

        public async Task RecordUserSearchAsync(int userId, string searchTerm, string? category, int resultCount)
        {
            var existingPattern = await _context.UserSearchPatterns
                .FirstOrDefaultAsync(p => p.UserId == userId &&
                                         p.SearchTerm == searchTerm &&
                                         p.Category == category);

            if (existingPattern != null)
            {
                existingPattern.SearchCount++;
                existingPattern.ResultCount = resultCount;
                existingPattern.LastSearched = DateTime.Now;
            }
            else
            {
                _context.UserSearchPatterns.Add(new UserSearchPattern
                {
                    UserId = userId,
                    SearchTerm = searchTerm,
                    Category = category,
                    SearchCount = 1,
                    ResultCount = resultCount,
                    LastSearched = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUserInterestsAsync(int userId)
        {
            var searchPatterns = await _context.UserSearchPatterns
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.SearchCount)
                .ThenByDescending(p => p.LastSearched)
                .Take(10)
                .ToListAsync();

            var interests = new List<string>();

            foreach (var pattern in searchPatterns)
            {
                if (!string.IsNullOrEmpty(pattern.Category))
                {
                    interests.Add(pattern.Category);
                }

                var terms = pattern.SearchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                interests.AddRange(terms);
            }

            return interests.Distinct().ToList();
        }

        private double CalculateRelevanceScore(LocalEvent localEvent, List<string> userInterests)
        {
            double score = 0;

            var eventText = $"{localEvent.Title} {localEvent.Description} {localEvent.Category}".ToLower();

            foreach (var interest in userInterests)
            {
                if (eventText.Contains(interest.ToLower()))
                {
                    score += 10;
                }
            }

            var daysUntilEvent = (localEvent.EventDate - DateTime.Now).TotalDays;
            if (daysUntilEvent <= 7)
            {
                score += 5;
            }
            else if (daysUntilEvent <= 14)
            {
                score += 3;
            }

            return score;
        }

        private string GetRecommendationReason(LocalEvent localEvent, List<string> userInterests)
        {
            var matchingInterests = userInterests
                .Where(interest =>
                    localEvent.Title.Contains(interest, StringComparison.OrdinalIgnoreCase) ||
                    localEvent.Description.Contains(interest, StringComparison.OrdinalIgnoreCase) ||
                    (localEvent.Category?.Contains(interest, StringComparison.OrdinalIgnoreCase) ?? false))
                .Take(3)
                .ToList();

            if (matchingInterests.Any())
            {
                return $"Based on your interest in: {string.Join(", ", matchingInterests)}";
            }

            return "Recommended for you";
        }
    }
}
