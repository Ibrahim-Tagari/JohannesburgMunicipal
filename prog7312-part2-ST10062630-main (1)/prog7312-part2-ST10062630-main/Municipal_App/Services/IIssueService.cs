using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    public interface IIssueService
    {
        // Core CRUD operations
        Task<List<Issue>> GetAllIssuesAsync();
        // Task<List<Issue>> GetUserIssuesAsync(int userId);
        Task<List<Issue>> GetUserIssuesAsync(int userId);

        Task<Issue?> GetIssueByIdAsync(int id);
        Task<Issue?> GetUserIssueByIdAsync(int id, int userId);
        Task<int> SubmitIssueAsync(Issue issue);
        Task<bool> UpdateIssueAsync(Issue issue);
        Task<bool> UpdateIssueStatusAsync(int issueId, string status, string? priority, string? assignedTo, string? updateNotes);
        Task<bool> DeleteIssueAsync(int id);

        // Search and filter
        Task<List<Issue>> SearchIssuesAsync(string? searchTerm, string? status = null, string? category = null, string? priority = null);
        Task<List<Issue>> SearchUserIssuesAsync(int userId, string? searchTerm, string? status, string? category);

        // Statistics
        Task<Dictionary<string, int>> GetIssueStatisticsAsync();
        Task<Dictionary<string, int>> GetUserIssueStatisticsAsync(int userId);

        // Reference data
        List<string> GetCategories();
        List<string> GetPriorities();
        List<string> GetStatuses();
    }
}
