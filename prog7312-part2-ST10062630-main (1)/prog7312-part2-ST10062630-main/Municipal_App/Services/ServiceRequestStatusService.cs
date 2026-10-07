using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Municipal_App.Data;
using Municipal_App.Models;
using Municipal_App.Services.DataStructures;

namespace Municipal_App.Services
{
    /// <summary>
    /// Implements Service Request Status functionality using advanced data structures
    /// Demonstrates efficient use of BST, AVL, Red-Black Tree, Heap, Graph, and MST
    /// </summary>
    public class ServiceRequestStatusService : IServiceRequestStatusService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceRequestStatusService> _logger;

        // Data structures for efficient operations
        private readonly BinarySearchTree<Issue> _bst = new BinarySearchTree<Issue>();
        private readonly AVLTree<Issue> _avlTree = new AVLTree<Issue>();
        private readonly RedBlackTree<Issue> _rbTree = new RedBlackTree<Issue>();
        private readonly MinHeap<Issue> _priorityHeap = new MinHeap<Issue>();
        private readonly ServiceRequestGraph _dependencyGraph = new ServiceRequestGraph();
        private readonly MinimumSpanningTree _mst = new MinimumSpanningTree();

        public ServiceRequestStatusService(ApplicationDbContext context, ILogger<ServiceRequestStatusService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeDataStructuresAsync()
        {
            try
            {
                var issues = await _context.Issues
                    .Include(i => i.User)
                    .OrderByDescending(i => i.DateReported)
                    .ToListAsync();

                _logger.LogInformation($"Initializing data structures with {issues.Count} issues");

                // Populate Binary Search Tree (sorted by ID)
                foreach (var issue in issues)
                {
                    _bst.Insert(issue.Id, issue);
                }

                // Populate AVL Tree (sorted by priority score)
                foreach (var issue in issues)
                {
                    int priorityScore = CalculatePriorityScore(issue);
                    _avlTree.Insert(priorityScore, issue);
                }

                // Populate Red-Black Tree (sorted by days open)
                foreach (var issue in issues)
                {
                    _rbTree.Insert(issue.DaysOpen, issue);
                }

                // Populate Min-Heap (by priority score)
                foreach (var issue in issues)
                {
                    int priorityScore = CalculatePriorityScore(issue);
                    _priorityHeap.Insert(priorityScore, issue);
                }

                // Build dependency graph
                foreach (var issue in issues)
                {
                    _dependencyGraph.AddNode(issue.Id, issue);

                    // Create edges based on category similarity and location proximity
                    foreach (var relatedIssue in issues.Where(i => i.Id != issue.Id))
                    {
                        if (AreIssuesRelated(issue, relatedIssue))
                        {
                            _dependencyGraph.AddEdge(issue.Id, relatedIssue.Id);
                        }
                    }
                }

                _logger.LogInformation("Data structures initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing data structures");
                throw;
            }
        }

        private int CalculatePriorityScore(Issue issue)
        {
            int score = 0;

            // Priority weighting
            score += issue.Priority switch
            {
                "Critical" => 100,
                "High" => 75,
                "Medium" => 50,
                "Low" => 25,
                _ => 10
            };

            // Status weighting (older statuses get higher priority)
            score += issue.Status switch
            {
                "Submitted" => 50,
                "Under Review" => 40,
                "Approved" => 30,
                "In Progress" => 20,
                "Resolved" => 0,
                _ => 10
            };

            // Age factor (older issues get higher priority)
            score += Math.Min(issue.DaysOpen * 2, 100);

            return score;
        }

        private bool AreIssuesRelated(Issue issue1, Issue issue2)
        {
            // Issues are related if they share the same category or nearby location
            if (issue1 == null || issue2 == null) return false;

            if (issue1.Category != null && issue1.Category == issue2.Category)
                return true;

            // Check if locations have common keywords
            if (string.IsNullOrEmpty(issue1.Location) || string.IsNullOrEmpty(issue2.Location))
                return false;

            var location1Words = issue1.Location.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var location2Words = issue2.Location.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return location1Words.Intersect(location2Words).Any();
        }

        public async Task<List<Issue>> GetAllServiceRequestsAsync()
        {
            return await _context.Issues
                .Include(i => i.User)
                .Where(i => i != null)
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();
        }

        public async Task<Issue?> GetServiceRequestByIdAsync(int id)
        {
            return await _context.Issues
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Issue>> SearchServiceRequestsAsync(string? searchTerm, string? status)
        {
            var query = _context.Issues.Include(i => i.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i =>
                    (i.Location != null && i.Location.Contains(searchTerm)) ||
                    (i.Description != null && i.Description.Contains(searchTerm)) ||
                    (i.Category != null && i.Category.Contains(searchTerm)) ||
                    i.Id.ToString().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(i => i.Status == status);
            }

            return await query.OrderByDescending(i => i.DateReported).ToListAsync();
        }

        public Task<List<Issue>> GetRequestsByPriorityRange(int minPriority, int maxPriority)
        {
            var results = _bst.GetRange(minPriority, maxPriority);
            return Task.FromResult(results);
        }

        public Task<Issue?> FindRequestByBST(int issueId)
        {
            var result = _bst.Search(issueId);
            return Task.FromResult(result);
        }

        public Task<Issue?> FindRequestByAVL(int issueId)
        {
            // Search by priority score
            var allIssues = _avlTree.InOrderTraversal();
            var result = allIssues.FirstOrDefault(i => i.Id == issueId);
            return Task.FromResult(result);
        }

        public Task<Issue?> FindRequestByRedBlack(int issueId)
        {
            var allIssues = _rbTree.InOrderTraversal();
            var result = allIssues.FirstOrDefault(i => i.Id == issueId);
            return Task.FromResult(result);
        }

        public Task<List<Issue>> GetTopPriorityRequests(int count)
        {
            var results = new List<Issue>();
            var heapCopy = new MinHeap<Issue>();

            // Copy current heap elements
            var allElements = _priorityHeap.GetAllElements();
            foreach (var issue in allElements)
            {
                heapCopy.Insert(CalculatePriorityScore(issue), issue);
            }

            // Extract top N
            for (int i = 0; i < count && heapCopy.Count > 0; i++)
            {
                var issue = heapCopy.ExtractMin();
                if (issue != null)
                    results.Add(issue);
            }

            return Task.FromResult(results);
        }

        public Task<Issue?> GetHighestPriorityRequest()
        {
            var result = _priorityHeap.PeekMin();
            return Task.FromResult(result);
        }

        public Task<List<Issue>> GetRelatedRequests(int issueId)
        {
            var results = _dependencyGraph.BreadthFirstSearch(issueId);
            return Task.FromResult(results);
        }

        public Task<List<Issue>> GetDependencyChain(int issueId)
        {
            var results = _dependencyGraph.DepthFirstSearch(issueId);
            return Task.FromResult(results);
        }

        public Task<bool> HasCircularDependencies()
        {
            var result = _dependencyGraph.HasCycle();
            return Task.FromResult(result);
        }

        public Task<Dictionary<int, List<int>>> GetDependencyGraph()
        {
            var result = _dependencyGraph.GetAdjacencyList();
            return Task.FromResult(result);
        }

        public async Task<List<Issue>> GetOptimalRequestClusters()
        {
            var issues = await GetAllServiceRequestsAsync();
            var edges = new List<MinimumSpanningTree.Edge>();

            // Create weighted edges between related issues
            for (int i = 0; i < issues.Count; i++)
            {
                for (int j = i + 1; j < issues.Count; j++)
                {
                    if (AreIssuesRelated(issues[i], issues[j]))
                    {
                        double weight = CalculateRelationshipWeight(issues[i], issues[j]);
                        edges.Add(new MinimumSpanningTree.Edge
                        {
                            Source = issues[i].Id,
                            Destination = issues[j].Id,
                            Weight = weight
                        });
                    }
                }
            }

            var mstEdges = _mst.FindMST(edges);
            var mstAdjList = _mst.GetMSTAsAdjacencyList(mstEdges);

            // Return issues that are part of the MST
            var issueIds = new HashSet<int>();
            foreach (var kvp in mstAdjList)
            {
                issueIds.Add(kvp.Key);
                issueIds.UnionWith(kvp.Value);
            }

            return issues.Where(i => issueIds.Contains(i.Id)).ToList();
        }

        public async Task<double> CalculateOptimalProcessingCost()
        {
            var issues = await GetAllServiceRequestsAsync();
            var edges = new List<MinimumSpanningTree.Edge>();

            // Create weighted edges
            for (int i = 0; i < issues.Count; i++)
            {
                for (int j = i + 1; j < issues.Count; j++)
                {
                    if (AreIssuesRelated(issues[i], issues[j]))
                    {
                        double weight = CalculateRelationshipWeight(issues[i], issues[j]);
                        edges.Add(new MinimumSpanningTree.Edge
                        {
                            Source = issues[i].Id,
                            Destination = issues[j].Id,
                            Weight = weight
                        });
                    }
                }
            }

            var mstEdges = _mst.FindMST(edges);
            return _mst.GetTotalWeight(mstEdges);
        }

        private double CalculateRelationshipWeight(Issue issue1, Issue issue2)
        {
            if (issue1 == null || issue2 == null) return 10.0;

            double weight = 10.0; // Base weight

            // Reduce weight if same category (stronger relationship)
            if (issue1.Category != null && issue1.Category == issue2.Category)
                weight -= 3.0;

            // Reduce weight if same priority
            if (issue1.Priority != null && issue1.Priority == issue2.Priority)
                weight -= 2.0;

            // Reduce weight based on proximity of reporting dates
            var daysDiff = Math.Abs((issue1.DateReported - issue2.DateReported).Days);
            weight -= Math.Max(0, 5 - daysDiff);

            return Math.Max(1.0, weight); // Ensure positive weight
        }

        public async Task<Dictionary<string, object>> GetDataStructureStatistics()
        {
            var issues = await GetAllServiceRequestsAsync();

            return new Dictionary<string, object>
            {
                ["TotalRequests"] = issues.Count,
                ["BSTNodes"] = issues.Count,
                ["AVLHeight"] = CalculateAVLHeight(),
                ["RedBlackNodes"] = issues.Count,
                ["HeapSize"] = _priorityHeap.Count,
                ["GraphNodes"] = _dependencyGraph.GetAdjacencyList().Count,
                ["GraphEdges"] = _dependencyGraph.GetAdjacencyList().Sum(kvp => kvp.Value.Count),
                ["HasCycles"] = await HasCircularDependencies(),
                ["OptimalCost"] = await CalculateOptimalProcessingCost()
            };
        }

        private int CalculateAVLHeight()
        {
            // Simplified height calculation
            var elements = _avlTree.InOrderTraversal();
            if (elements.Count == 0) return 0;
            return (int)Math.Ceiling(Math.Log2(elements.Count + 1));
        }
    }
}
