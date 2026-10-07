using Microsoft.AspNetCore.Mvc;
using Municipal_App.Models;
using Municipal_App.Services;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Municipal_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIssueService _issueService;
        private readonly ILocalEventService _eventService;

        public HomeController(
            ILogger<HomeController> logger,
            IIssueService issueService,
            ILocalEventService eventService)
        {
            _logger = logger;
            _issueService = issueService;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get issue statistics
                var issueStatistics = await _issueService.GetIssueStatisticsAsync();
                ViewBag.IssueStatistics = issueStatistics;

                // Get upcoming events count
                var upcomingEvents = await _eventService.GetUpcomingEventsAsync(5);
                ViewBag.UpcomingEventsCount = upcomingEvents.Count;
                ViewBag.UpcomingEvents = upcomingEvents;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                ViewBag.IssueStatistics = new Dictionary<string, int>();
                ViewBag.UpcomingEventsCount = 0;
                ViewBag.UpcomingEvents = new List<LocalEvent>();
                return View();
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View(new ContactFormModel());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
