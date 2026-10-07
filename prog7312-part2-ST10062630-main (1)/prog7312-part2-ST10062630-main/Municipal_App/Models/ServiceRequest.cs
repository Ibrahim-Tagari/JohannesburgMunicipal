using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipal_App.Models
{
    /// <summary>
    /// Service Request model that extends Issue with graph and tree properties
    /// Used for Part 3 advanced data structures implementation
    /// </summary>
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IssueId { get; set; }

        [ForeignKey("IssueId")]
        public virtual Issue? Issue { get; set; }

        // Graph properties for dependency management
        public List<int> DependsOn { get; set; } = new List<int>();
        public List<int> BlockedBy { get; set; } = new List<int>();

        // Priority for heap operations
        public int PriorityScore { get; set; }

        // Tree traversal properties
        public DateTime ProcessStarted { get; set; }
        public DateTime? ProcessCompleted { get; set; }

        // Graph edge weights (for MST calculations)
        public Dictionary<int, double> RelatedRequests { get; set; } = new Dictionary<int, double>();

        // Status tracking
        public string CurrentStage { get; set; } = "Submitted";
        public int EstimatedDays { get; set; }

        [NotMapped]
        public double CompletionPercentage
        {
            get
            {
                return CurrentStage switch
                {
                    "Submitted" => 10,
                    "Under Review" => 25,
                    "Approved" => 50,
                    "In Progress" => 75,
                    "Resolved" => 100,
                    _ => 0
                };
            }
        }
    }
}
