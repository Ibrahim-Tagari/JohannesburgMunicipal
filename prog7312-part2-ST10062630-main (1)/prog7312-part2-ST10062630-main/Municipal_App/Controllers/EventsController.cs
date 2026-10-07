using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municipal_App.Models;
using Municipal_App.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Municipal_App.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ILocalEventService _eventService;
        private readonly IEventDataStructureService _dataStructureService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(
            ILocalEventService eventService,
            IEventDataStructureService dataStructureService,
            ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _dataStructureService = dataStructureService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm = "", string? category = "", string? sortBy = "date", DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var allEvents = await _eventService.GetAllEventsAsync();

                _dataStructureService.LoadEvents(allEvents);

                var filteredEvents = await _eventService.SearchEventsAsync(
                    searchTerm,
                    string.IsNullOrEmpty(category) ? null : category,
                    startDate,
                    endDate
                );

                var categories = _dataStructureService.GetUniqueCategories().OrderBy(c => c).ToList();
                var eventsByCategory = _dataStructureService.GetEventsByCategory();
                var eventsByDate = _dataStructureService.GetEventsByDate();
                var eventStats = await _eventService.GetEventCountByCategoryAsync();

                var sortedEvents = sortBy switch
                {
                    "priority" => _dataStructureService.GetEventsByPriority().Where(e => filteredEvents.Any(f => f.Id == e.Id)).ToList(),
                    "category" => filteredEvents.OrderBy(e => e.Category).ThenBy(e => e.EventDate).ToList(),
                    "date" => filteredEvents.OrderBy(e => e.EventDate).ToList(),
                    _ => filteredEvents.OrderBy(e => e.EventDate).ToList()
                };

                ViewBag.SearchTerm = searchTerm;
                ViewBag.SelectedCategory = category;
                ViewBag.SortBy = sortBy;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                ViewBag.Categories = categories;
                ViewBag.EventsByCategory = eventsByCategory;
                ViewBag.EventsByDate = eventsByDate;
                ViewBag.EventStats = eventStats;
                ViewBag.TotalEvents = allEvents.Count;
                ViewBag.FilteredCount = sortedEvents.Count;
                ViewBag.UniqueCategories = categories.Count;
                ViewBag.UniqueDates = _dataStructureService.GetUniqueDates().Count;

                return View(sortedEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading events index");
                TempData["ErrorMessage"] = "Error loading events. Please try again.";
                return View(new System.Collections.Generic.List<LocalEvent>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var evt = await _eventService.GetEventByIdAsync(id);
                if (evt == null)
                {
                    TempData["ErrorMessage"] = "Event not found.";
                    return RedirectToAction("Index");
                }

                await _eventService.IncrementViewCountAsync(id);

                var relatedEvents = await _eventService.SearchEventsAsync("", evt.Category, null, null);
                relatedEvents = relatedEvents.Where(e => e.Id != id).Take(4).ToList();

                ViewBag.RelatedEvents = relatedEvents;

                return View(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading event details for event {EventId}", id);
                TempData["ErrorMessage"] = "Error loading event details.";
                return RedirectToAction("Index");
            }
        }
    }
}
