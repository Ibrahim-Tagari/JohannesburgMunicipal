using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Municipal_App.Models
{
    public class UpdateIssueStatusViewModel
    {
        public int Id { get; set; }
        public int IssueId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;

        public string? AssignedTo { get; set; }
        public string? UpdateNotes { get; set; }

        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateReported { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public List<string> Statuses { get; set; } = new List<string>();
        public List<string> Priorities { get; set; } = new List<string>();
    }
}
