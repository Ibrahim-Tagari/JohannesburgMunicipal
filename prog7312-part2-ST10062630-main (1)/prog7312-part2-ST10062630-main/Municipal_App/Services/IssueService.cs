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
    public class IssueService : IIssueService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IssueService> _logger;

        public IssueService(ApplicationDbContext context, ILogger<IssueService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Issue>> GetAllIssuesAsync()
        {
            return await _context.Issues
                .Include(i => i.User)
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();
        }

        public async Task<List<Issue>> GetUserIssuesAsync(int userId)
        {
            return await _context.Issues
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();
        }

        public async Task<Issue?> GetIssueByIdAsync(int id)
        {
            return await _context.Issues
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Issue?> GetUserIssueByIdAsync(int id, int userId)
        {
            return await _context.Issues
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
        }

        public async Task<int> SubmitIssueAsync(Issue issue)
        {
            try
            {
                if (issue == null)
                    throw new ArgumentNullException(nameof(issue));

                _context.Issues.Add(issue);
                await _context.SaveChangesAsync();

                return issue.Id; // Ensure your Issue model has a primary key property called Id
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving issue to database");
                throw;
            }
        }


        public async Task<bool> UpdateIssueAsync(Issue issue)
        {
            try
            {
                _context.Entry(issue).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue {IssueId}", issue.Id);
                return false;
            }
        }

        public async Task<bool> UpdateIssueStatusAsync(int issueId, string status, string? priority, string? assignedTo, string? updateNotes)
        {
            try
            {
                var issue = await GetIssueByIdAsync(issueId);
                if (issue == null) return false;

                issue.Status = status;
                if (priority != null) issue.Priority = priority;
                if (assignedTo != null) issue.AssignedTo = assignedTo;
                if (updateNotes != null) issue.UpdateNotes = updateNotes;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue status for {IssueId}", issueId);
                return false;
            }
        }

        public async Task<bool> DeleteIssueAsync(int id)
        {
            var issue = await GetIssueByIdAsync(id);
            if (issue == null) return false;

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Issue>> SearchIssuesAsync(string? searchTerm, string? status = null, string? category = null, string? priority = null)
        {
            var query = _context.Issues.Include(i => i.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i =>
                    i.Location.Contains(searchTerm) ||
                    i.Description.Contains(searchTerm) ||
                    i.Id.ToString().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(i => i.Category == category);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(i => i.Priority == priority);

            return await query.OrderByDescending(i => i.DateReported).ToListAsync();
        }

        public async Task<List<Issue>> SearchUserIssuesAsync(int userId, string? searchTerm, string? status, string? category)
        {
            var query = _context.Issues
                .Where(i => i.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i =>
                    i.Location.Contains(searchTerm) ||
                    i.Description.Contains(searchTerm) ||
                    i.Id.ToString().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(i => i.Category == category);

            return await query.OrderByDescending(i => i.DateReported).ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetIssueStatisticsAsync()
        {
            var stats = new Dictionary<string, int>
            {
                ["Total"] = await _context.Issues.CountAsync(),
                ["Submitted"] = await _context.Issues.CountAsync(i => i.Status == "Submitted"),
                ["In Progress"] = await _context.Issues.CountAsync(i => i.Status == "In Progress"),
                ["Resolved"] = await _context.Issues.CountAsync(i => i.Status == "Resolved"),
                ["Closed"] = await _context.Issues.CountAsync(i => i.Status == "Closed")
            };
            return stats;
        }

        public async Task<Dictionary<string, int>> GetUserIssueStatisticsAsync(int userId)
        {
            var stats = new Dictionary<string, int>
            {
                ["Total"] = await _context.Issues.CountAsync(i => i.UserId == userId),
                ["Submitted"] = await _context.Issues.CountAsync(i => i.UserId == userId && i.Status == "Submitted"),
                ["In Progress"] = await _context.Issues.CountAsync(i => i.UserId == userId && i.Status == "In Progress"),
                ["Resolved"] = await _context.Issues.CountAsync(i => i.UserId == userId && i.Status == "Resolved"),
                ["Closed"] = await _context.Issues.CountAsync(i => i.UserId == userId && i.Status == "Closed")
            };
            return stats;
        }

        public List<string> GetCategories()
        {
            return new List<string>
            {
                "Roads & Infrastructure",
                "Water & Sanitation",
                "Electricity",
                "Waste Management",
                "Public Safety",
                "Parks & Recreation",
                "Housing",
                "Other"
            };
        }

        public List<string> GetPriorities()
        {
            return new List<string> { "Low", "Medium", "High", "Critical" };
        }

        public List<string> GetStatuses()
        {
            return new List<string> { "Submitted", "In Progress", "Resolved", "Closed", "Cancelled" };
        }
    }
}
