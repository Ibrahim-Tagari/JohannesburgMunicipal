using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipal_App.Models
{
    /// <summary>
    /// Represents a user account in the municipal services system
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [Display(Name = "Username")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username can only contain letters, numbers, dots, underscores, and hyphens")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password hash is required")]
        [StringLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        // For passing password during signup
        [NotMapped]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(20)]
        [Display(Name = "Role")]
        public string Role { get; set; } = "User";

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        [StringLength(200)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(50)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Navigation property for issues reported by this user
        /// </summary>
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();

        /// <summary>
        /// Navigation property for notifications sent to this user
        /// </summary>
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        /// <summary>
        /// Navigation property for push subscriptions for this user
        /// </summary>
        public virtual ICollection<PushSubscription> PushSubscriptions { get; set; } = new List<PushSubscription>();

        /// <summary>
        /// Navigation property for search patterns associated with this user
        /// </summary>
        public virtual ICollection<UserSearchPattern> SearchPatterns { get; set; } = new List<UserSearchPattern>();

        /// <summary>
        /// Gets the total number of issues reported by this user
        /// </summary>
        [NotMapped]
        public int TotalIssuesReported => Issues?.Count ?? 0;

        /// <summary>
        /// Gets the number of active issues for this user
        /// </summary>
        [NotMapped]
        public int ActiveIssues => Issues?.Count(i => i.Status != "Resolved" && i.Status != "Closed") ?? 0;
    }

    /// <summary>
    /// Enumeration of user roles in the system
    /// </summary>
    public enum UserRole
    {
        [Display(Name = "User")]
        User,

        [Display(Name = "Administrator")]
        Admin,

        [Display(Name = "Department Manager")]
        Manager,

        [Display(Name = "Technician")]
        Technician
    }
}
