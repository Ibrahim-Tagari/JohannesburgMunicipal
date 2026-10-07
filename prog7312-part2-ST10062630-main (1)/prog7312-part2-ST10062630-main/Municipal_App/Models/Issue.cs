using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipal_App.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Submitted";

        [Required]
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        public List<string> AttachedFiles { get; set; } = new List<string>();

        public string? ImagePath { get; set; }

        [Required]
        public DateTime DateReported { get; set; } = DateTime.Now;

        public DateTime? DateResolved { get; set; }

        public DateTime? LastUpdated { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? ContactEmail { get; set; }

        [Phone]
        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [StringLength(100)]
        public string? AssignedTo { get; set; }

        public string? UpdateNotes { get; set; }

        public DateTime? EstimatedResolutionDate { get; set; }

        public DateTime? ResolutionDate { get; set; }

        [NotMapped]
        public int DaysOpen => (DateTime.Now - DateReported).Days;

        [NotMapped]
        public bool IsOverdue => EstimatedResolutionDate.HasValue &&
                                 DateTime.Now > EstimatedResolutionDate.Value &&
                                 Status != "Resolved" && Status != "Closed";
    }
}
