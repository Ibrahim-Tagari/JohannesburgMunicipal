using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Municipal_App.Models
{
    /// <summary>
    /// View model for displaying the issue log with filters and statistics
    /// </summary>
    public class IssueLogViewModel
    {
        /// <summary>
        /// List of issues to display
        /// </summary>
        public List<Issue> Issues { get; set; } = new List<Issue>();

        /// <summary>
        /// Statistical data for dashboard
        /// </summary>
        public Dictionary<string, int> Statistics { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Available categories for filtering
        /// </summary>
        public List<string> Categories { get; set; } = new List<string>();

        /// <summary>
        /// Available statuses for filtering
        /// </summary>
        public List<string> Statuses { get; set; } = new List<string>();

        /// <summary>
        /// Current filter: search term
        /// </summary>
        [Display(Name = "Search")]
        public string? SearchQuery { get; set; }

        /// <summary>
        /// Current filter: status
        /// </summary>
        [Display(Name = "Status")]
        public string StatusFilter { get; set; } = "All";

        /// <summary>
        /// Current filter: category
        /// </summary>
        [Display(Name = "Category")]
        public string CategoryFilter { get; set; } = "All";

        /// <summary>
        /// Current filter: priority
        /// </summary>
        [Display(Name = "Priority")]
        public string? PriorityFilter { get; set; }

        /// <summary>
        /// Current filter: search term
        /// </summary>
        [Display(Name = "Search Term")]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Current filter: date from
        /// </summary>
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Current filter: date to
        /// </summary>
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Current sorting field
        /// </summary>
        public string SortBy { get; set; } = "DateReported";

        /// <summary>
        /// Current sorting direction
        /// </summary>
        public bool SortDescending { get; set; } = true;

        /// <summary>
        /// Current page number for pagination
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Items per page
        /// </summary>
        public int ItemsPerPage { get; set; } = 10;

        /// <summary>
        /// Total number of issues (before pagination)
        /// </summary>
        public int TotalIssues { get; set; } = 0;

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalIssues / ItemsPerPage);

        /// <summary>
        /// Chart data for visualizations
        /// </summary>
        public Dictionary<string, object> ChartData { get; set; } = new Dictionary<string, object>();
    }
}
