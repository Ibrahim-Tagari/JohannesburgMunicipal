using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Municipal_App.Models;
using Municipal_App.Services;  // Changed from Municipal_APP to Municipal_App
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Municipal_App.Controllers
{
    [Authorize]
    public class IssuesController : Controller
    {
        private readonly IIssueService _issueService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<IssuesController> _logger;

        public IssuesController(IIssueService issueService, IWebHostEnvironment environment, ILogger<IssuesController> logger)
        {
            _issueService = issueService;
            _environment = environment;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Unable to get current user ID from claims");
                return 0;
            }
            return userId;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        public IActionResult Report()
        {
            var model = new ReportIssueViewModel
            {
                Categories = _issueService.GetCategories(),
                Priorities = _issueService.GetPriorities()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(ReportIssueViewModel model)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    _logger.LogError("Unable to determine current user ID for issue submission");
                    TempData["ErrorMessage"] = "Authentication error. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                _logger.LogInformation("Attempting to submit issue for user {UserId}", currentUserId);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model validation failed for issue submission");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                    }

                    model.Categories = _issueService.GetCategories();
                    model.Priorities = _issueService.GetPriorities();
                    return View(model);
                }

                var issue = new Issue
                {
                    UserId = currentUserId,
                    Location = model.Location?.Trim() ?? string.Empty,
                    Category = model.Category?.Trim() ?? string.Empty,
                    Description = model.Description?.Trim() ?? string.Empty,
                    Priority = !string.IsNullOrEmpty(model.Priority) ? model.Priority.Trim() : "Medium",
                    ContactEmail = model.ContactEmail?.Trim(),
                    ContactPhone = model.ContactPhone?.Trim(),
                    AttachedFiles = new List<string>(),
                    DateReported = DateTime.Now,
                    Status = "Submitted"
                };

                _logger.LogInformation("Created issue object: UserId={UserId}, Location={Location}, Category={Category}",
                    issue.UserId, issue.Location, issue.Category);

                // Handle file uploads
                if (model.AttachedFiles != null && model.AttachedFiles.Any())
                {
                    _logger.LogInformation("Processing {FileCount} attached files", model.AttachedFiles.Count);

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                        _logger.LogInformation("Created uploads directory: {Path}", uploadsFolder);
                    }

                    foreach (var file in model.AttachedFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            try
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                issue.AttachedFiles.Add(fileName);
                                _logger.LogInformation("Successfully uploaded file: {FileName}", fileName);
                            }
                            catch (Exception fileEx)
                            {
                                _logger.LogError(fileEx, "Error uploading file: {FileName}", file.FileName);
                            }
                        }
                    }
                }

                var issueId = await _issueService.SubmitIssueAsync(issue);
                _logger.LogInformation("Issue submitted successfully with ID: {IssueId}", issueId);

                TempData["SuccessMessage"] = $"Issue reported successfully! Reference ID: {issueId}";
                return RedirectToAction("ReportSuccess", new { id = issueId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting issue report for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while submitting your report. Please try again.";

                model.Categories = _issueService.GetCategories();
                model.Priorities = _issueService.GetPriorities();
                return View(model);
            }
        }

        public async Task<IActionResult> ReportSuccess(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                Issue? issue;

                if (IsAdmin())
                {
                    issue = await _issueService.GetIssueByIdAsync(id);
                }
                else
                {
                    issue = await _issueService.GetUserIssueByIdAsync(id, currentUserId);
                }

                if (issue == null)
                {
                    _logger.LogWarning("Issue {IssueId} not found for user {UserId}", id, currentUserId);
                    TempData["ErrorMessage"] = "Issue not found.";
                    return RedirectToAction("TrackIssue");
                }

                return View(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving issue {IssueId} for success page", id);
                TempData["ErrorMessage"] = "Error loading issue details.";
                return RedirectToAction("TrackIssue");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IssueLog()
        {
            try
            {
                var issues = await _issueService.GetAllIssuesAsync();
                var statistics = await _issueService.GetIssueStatisticsAsync();

                var viewModel = new IssueLogViewModel
                {
                    Issues = issues,
                    Statistics = statistics,
                    Categories = _issueService.GetCategories(),
                    Statuses = _issueService.GetStatuses()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading issue log");
                TempData["ErrorMessage"] = "Error loading issue log. Please try again.";
                return View(new IssueLogViewModel
                {
                    Issues = new List<Issue>(),
                    Statistics = new Dictionary<string, int>(),
                    Categories = _issueService.GetCategories(),
                    Statuses = _issueService.GetStatuses()
                });
            }
        }

        public async Task<IActionResult> TrackIssue(int? id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (id.HasValue)
                {
                    Issue? issue;

                    if (IsAdmin())
                    {
                        issue = await _issueService.GetIssueByIdAsync(id.Value);
                    }
                    else
                    {
                        issue = await _issueService.GetUserIssueByIdAsync(id.Value, currentUserId);
                    }

                    if (issue != null)
                    {
                        return View("TrackIssueDetails", issue);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Issue with ID {id} not found or you don't have permission to view it.";
                    }
                }

                List<Issue> issues;
                Dictionary<string, int> statistics;

                if (IsAdmin())
                {
                    issues = await _issueService.GetAllIssuesAsync();
                    statistics = await _issueService.GetIssueStatisticsAsync();
                }
                else
                {
                    issues = await _issueService.GetUserIssuesAsync(currentUserId);
                    statistics = await _issueService.GetUserIssueStatisticsAsync(currentUserId);
                }

                var viewModel = new IssueLogViewModel
                {
                    Issues = issues,
                    Statistics = statistics,
                    Categories = _issueService.GetCategories(),
                    Statuses = _issueService.GetStatuses()
                };

                return View("TrackIssue", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading track issue page");
                TempData["ErrorMessage"] = "Error loading your issues. Please try again.";
                return View("TrackIssue", new IssueLogViewModel
                {
                    Issues = new List<Issue>(),
                    Statistics = new Dictionary<string, int>(),
                    Categories = _issueService.GetCategories(),
                    Statuses = _issueService.GetStatuses()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TrackIssueById(int issueId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                Issue? issue;

                if (IsAdmin())
                {
                    issue = await _issueService.GetIssueByIdAsync(issueId);
                }
                else
                {
                    issue = await _issueService.GetUserIssueByIdAsync(issueId, currentUserId);
                }

                if (issue == null)
                {
                    TempData["ErrorMessage"] = $"Issue with ID {issueId} not found or you don't have permission to view it.";
                    return RedirectToAction("TrackIssue");
                }

                return View("TrackIssueDetails", issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking issue {IssueId}", issueId);
                TempData["ErrorMessage"] = "Error retrieving issue details. Please try again.";
                return RedirectToAction("TrackIssue");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetIssueDetails(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return Unauthorized();
                }

                Issue? issue;

                if (IsAdmin())
                {
                    issue = await _issueService.GetIssueByIdAsync(id);
                }
                else
                {
                    issue = await _issueService.GetUserIssueByIdAsync(id, currentUserId);
                }

                if (issue == null)
                {
                    return NotFound();
                }

                return PartialView("_IssueDetailsPartial", issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue details for {IssueId}", id);
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchIssues(string searchTerm, string status, string category)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return PartialView("_IssueCardsPartial", new List<Issue>());
                }

                List<Issue> issues;

                if (IsAdmin())
                {
                    issues = await _issueService.GetAllIssuesAsync();

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        issues = issues.Where(i =>
                            i.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            i.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            i.Id.ToString().Contains(searchTerm)).ToList();
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        issues = issues.Where(i => i.Status == status).ToList();
                    }

                    if (!string.IsNullOrEmpty(category))
                    {
                        issues = issues.Where(i => i.Category == category).ToList();
                    }
                }
                else
                {
                    issues = await _issueService.SearchUserIssuesAsync(currentUserId, searchTerm, status, category);
                }

                return PartialView("_IssueCardsPartial", issues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching issues");
                return PartialView("_IssueCardsPartial", new List<Issue>());
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                if (issue == null)
                {
                    return NotFound();
                }

                var viewModel = new UpdateIssueStatusViewModel
                {
                    Id = issue.Id,
                    IssueId = issue.Id,
                    Status = issue.Status,
                    Priority = issue.Priority,
                    AssignedTo = issue.AssignedTo,
                    UpdateNotes = issue.UpdateNotes,
                    Location = issue.Location,
                    Category = issue.Category,
                    Description = issue.Description,
                    DateReported = issue.DateReported,
                    ContactEmail = issue.ContactEmail,
                    ContactPhone = issue.ContactPhone,
                    Statuses = _issueService.GetStatuses(),
                    Priorities = _issueService.GetPriorities()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading update status page for issue {IssueId}", id);
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateIssueStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Statuses = _issueService.GetStatuses();
                model.Priorities = _issueService.GetPriorities();
                return View(model);
            }

            try
            {
                var success = await _issueService.UpdateIssueStatusAsync(
                    model.Id,
                    model.Status,
                    model.Priority,
                    model.AssignedTo,
                    model.UpdateNotes);

                if (success)
                {
                    TempData["SuccessMessage"] = "Issue status updated successfully!";
                    return RedirectToAction("IssueLog");
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the issue status.";
                    model.Statuses = _issueService.GetStatuses();
                    model.Priorities = _issueService.GetPriorities();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue status for {IssueId}", model.Id);
                TempData["ErrorMessage"] = "An error occurred while updating the issue status.";
                model.Statuses = _issueService.GetStatuses();
                model.Priorities = _issueService.GetPriorities();
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                if (issue == null)
                {
                    TempData["ErrorMessage"] = "Issue not found.";
                    return RedirectToAction("IssueLog");
                }

                var model = new ReportIssueViewModel
                {
                    Id = issue.Id,
                    Location = issue.Location,
                    Category = issue.Category,
                    Description = issue.Description,
                    Priority = issue.Priority,
                    ContactEmail = issue.ContactEmail,
                    ContactPhone = issue.ContactPhone,
                    Categories = _issueService.GetCategories(),
                    Priorities = _issueService.GetPriorities()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit page for issue {IssueId}", id);
                TempData["ErrorMessage"] = "Error loading issue for editing.";
                return RedirectToAction("IssueLog");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReportIssueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _issueService.GetCategories();
                model.Priorities = _issueService.GetPriorities();
                return View(model);
            }

            try
            {
                var issue = await _issueService.GetIssueByIdAsync(model.Id);
                if (issue == null)
                {
                    TempData["ErrorMessage"] = "Issue not found.";
                    return RedirectToAction("IssueLog");
                }

                issue.Location = model.Location?.Trim() ?? string.Empty;
                issue.Category = model.Category?.Trim() ?? string.Empty;
                issue.Description = model.Description?.Trim() ?? string.Empty;
                issue.Priority = model.Priority?.Trim() ?? "Medium";
                issue.ContactEmail = model.ContactEmail?.Trim();
                issue.ContactPhone = model.ContactPhone?.Trim();
                issue.LastUpdated = DateTime.Now;

                // Handle new file uploads if any
                if (model.AttachedFiles != null && model.AttachedFiles.Any())
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in model.AttachedFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            try
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                issue.AttachedFiles.Add(fileName);
                            }
                            catch (Exception fileEx)
                            {
                                _logger.LogError(fileEx, "Error uploading file during edit: {FileName}", file.FileName);
                            }
                        }
                    }
                }

                var success = await _issueService.UpdateIssueAsync(issue);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Issue #{issue.Id} updated successfully!";
                    return RedirectToAction("IssueLog");
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the issue.";
                    model.Categories = _issueService.GetCategories();
                    model.Priorities = _issueService.GetPriorities();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue {IssueId}", model.Id);
                TempData["ErrorMessage"] = "An error occurred while updating the issue.";
                model.Categories = _issueService.GetCategories();
                model.Priorities = _issueService.GetPriorities();
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                if (issue == null)
                {
                    TempData["ErrorMessage"] = "Issue not found.";
                    return RedirectToAction("IssueLog");
                }

                // Delete attached files if any
                if (issue.AttachedFiles != null && issue.AttachedFiles.Any())
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    foreach (var fileName in issue.AttachedFiles)
                    {
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            try
                            {
                                System.IO.File.Delete(filePath);
                            }
                            catch (Exception fileEx)
                            {
                                _logger.LogError(fileEx, "Error deleting file {FileName}", fileName);
                            }
                        }
                    }
                }

                var success = await _issueService.DeleteIssueAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Issue #{id} deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the issue.";
                }

                return RedirectToAction("IssueLog");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting issue {IssueId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the issue.";
                return RedirectToAction("IssueLog");
            }
        }
    }
}
