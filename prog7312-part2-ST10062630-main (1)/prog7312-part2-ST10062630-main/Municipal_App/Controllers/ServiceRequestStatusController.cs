using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Municipal_App.Services;

namespace Municipal_App.Controllers
{
    /// <summary>
    /// Controller for Service Request Status (Part 3)
    /// Demonstrates advanced data structures: BST, AVL, Red-Black Tree, Heap, Graph, MST
    /// </summary>
    [Authorize]
    public class ServiceRequestStatusController : Controller
    {
        private readonly IServiceRequestStatusService _statusService;
        private readonly ILogger<ServiceRequestStatusController> _logger;

        public ServiceRequestStatusController(
            IServiceRequestStatusService statusService,
            ILogger<ServiceRequestStatusController> logger)
        {
            _statusService = statusService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm = "", string? status = "All")
        {
            try
            {
                // Initialize data structures on first load
                await _statusService.InitializeDataStructuresAsync();

                var requests = await _statusService.SearchServiceRequestsAsync(searchTerm, status);
                var statistics = await _statusService.GetDataStructureStatistics();

                ViewBag.SearchTerm = searchTerm;
                ViewBag.StatusFilter = status;
                ViewBag.Statistics = statistics;
                ViewBag.TotalRequests = requests.Count;

                return View(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading service request status");
                TempData["ErrorMessage"] = "Error loading service requests.";
                return View(new System.Collections.Generic.List<Municipal_App.Models.Issue>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var request = await _statusService.GetServiceRequestByIdAsync(id);
                if (request == null)
                {
                    TempData["ErrorMessage"] = "Service request not found.";
                    return RedirectToAction("Index");
                }

                // Get related requests using graph traversal
                var relatedRequests = await _statusService.GetRelatedRequests(id);
                var dependencyChain = await _statusService.GetDependencyChain(id);

                ViewBag.RelatedRequests = relatedRequests.Take(5).ToList();
                ViewBag.DependencyChain = dependencyChain.Take(5).ToList();

                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading request details for {RequestId}", id);
                TempData["ErrorMessage"] = "Error loading request details.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PriorityQueue()
        {
            try
            {
                await _statusService.InitializeDataStructuresAsync();
                var topPriority = await _statusService.GetTopPriorityRequests(10);

                ViewBag.Title = "Priority Queue (Min-Heap)";
                ViewBag.Description = "Service requests ordered by priority using Min-Heap data structure";

                return View(topPriority);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading priority queue");
                TempData["ErrorMessage"] = "Error loading priority queue.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DependencyGraph()
        {
            try
            {
                await _statusService.InitializeDataStructuresAsync();
                var graph = await _statusService.GetDependencyGraph();
                var hasCycles = await _statusService.HasCircularDependencies();
                var allRequests = await _statusService.GetAllServiceRequestsAsync();

                ViewBag.Graph = graph;
                ViewBag.HasCycles = hasCycles;
                ViewBag.AllRequests = allRequests;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dependency graph");
                TempData["ErrorMessage"] = "Error loading dependency graph.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OptimalClusters()
        {
            try
            {
                await _statusService.InitializeDataStructuresAsync();
                var clusters = await _statusService.GetOptimalRequestClusters();
                var optimalCost = await _statusService.CalculateOptimalProcessingCost();

                ViewBag.OptimalCost = optimalCost;
                ViewBag.Title = "Optimal Request Clusters (MST)";
                ViewBag.Description = "Related service requests clustered using Minimum Spanning Tree algorithm";

                return View(clusters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading optimal clusters");
                TempData["ErrorMessage"] = "Error loading optimal clusters.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DataStructures()
        {
            try
            {
                await _statusService.InitializeDataStructuresAsync();
                var statistics = await _statusService.GetDataStructureStatistics();

                return View(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading data structure statistics");
                TempData["ErrorMessage"] = "Error loading statistics.";
                return RedirectToAction("Index");
            }
        }
    }
}
