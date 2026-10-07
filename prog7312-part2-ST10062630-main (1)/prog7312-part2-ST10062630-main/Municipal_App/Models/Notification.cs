using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipal_App.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "Info";

        public string? Icon { get; set; }

        [ForeignKey("RelatedIssue")]
        public int? RelatedId { get; set; }

        public string? Link { get; set; }

        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(200)]
        public string? ActionUrl { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;

        public virtual Issue? RelatedIssue { get; set; }

        [StringLength(20)]
        [Display(Name = "Priority")]
        public string Priority { get; set; } = "Normal";

        public string? TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;
                if (timeSpan.TotalMinutes < 1) return "Just now";
                if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
                if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
                if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} days ago";
                return CreatedAt.ToString("MMM dd, yyyy");
            }
        }

        public DateTime? ReadAt { get; set; }
    }
}
