using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Municipal_App.Models
{
    /// <summary>
    /// View model for reporting a new issue
    /// Includes validation and UI-specific properties
    /// </summary>
    public class ReportIssueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        [Display(Name = "Location of Issue")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Issue Category")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10)]
        [Display(Name = "Detailed Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority Level")]
        public string Priority { get; set; } = "Medium";

        [Display(Name = "Attach Files (Images/Documents)")]
        [DataType(DataType.Upload)]
        public List<IFormFile>? AttachedFiles { get; set; }

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [Required(ErrorMessage = "Contact phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Contact Phone")]
        public string? ContactPhone { get; set; }

        /// <summary>
        /// GPS Latitude coordinate if available
        /// </summary>
        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        /// <summary>
        /// GPS Longitude coordinate if available
        /// </summary>
        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        /// <summary>
        /// Indicates if the issue is urgent
        /// </summary>
        [Display(Name = "Mark as Urgent")]
        public bool IsUrgent { get; set; } = false;

        /// <summary>
        /// Form completion progress percentage
        /// </summary>
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// Available categories for dropdown
        /// </summary>
        public List<string> Categories { get; set; } = new List<string>();

        /// <summary>
        /// Available priorities for dropdown
        /// </summary>
        public List<string> Priorities { get; set; } = new List<string>();

        /// <summary>
        /// Terms and conditions acceptance
        /// </summary>
        [Display(Name = "I agree to the terms and conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
        public bool AcceptTerms { get; set; } = false;
    }
}
