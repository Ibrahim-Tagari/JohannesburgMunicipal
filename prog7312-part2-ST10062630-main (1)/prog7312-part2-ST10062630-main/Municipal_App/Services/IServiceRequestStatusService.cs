using System.Collections.Generic;
using System.Threading.Tasks;
using Municipal_App.Models;

namespace Municipal_App.Services
{
    /// <summary>
    /// Service interface for managing service request status with advanced data structures
    /// Implements BST, AVL, Red-Black Tree, Heap, Graph, and MST for efficient operations
    /// </summary>
    public interface IServiceRequestStatusService
    {
        // Core operations
        Task InitializeDataStructuresAsync();
        Task<List<Issue>> GetAllServiceRequestsAsync();
        Task<Issue?> GetServiceRequestByIdAsync(int id);
        Task<List<Issue>> SearchServiceRequestsAsync(string? searchTerm, string? status);

        // Tree-based operations
        Task<List<Issue>> GetRequestsByPriorityRange(int minPriority, int maxPriority);
        Task<Issue?> FindRequestByBST(int issueId);
        Task<Issue?> FindRequestByAVL(int issueId);
        Task<Issue?> FindRequestByRedBlack(int issueId);

        // Heap-based operations
        Task<List<Issue>> GetTopPriorityRequests(int count);
        Task<Issue?> GetHighestPriorityRequest();

        // Graph-based operations
        Task<List<Issue>> GetRelatedRequests(int issueId);
        Task<List<Issue>> GetDependencyChain(int issueId);
        Task<bool> HasCircularDependencies();
        Task<Dictionary<int, List<int>>> GetDependencyGraph();

        // MST operations
        Task<List<Issue>> GetOptimalRequestClusters();
        Task<double> CalculateOptimalProcessingCost();

        // Statistics
        Task<Dictionary<string, object>> GetDataStructureStatistics();
    }
}
