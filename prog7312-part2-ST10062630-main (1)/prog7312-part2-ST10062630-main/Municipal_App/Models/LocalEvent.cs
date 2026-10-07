using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municipal_App.Models
{
    public class LocalEvent
    {
        [Key]
        public int Id { get; set; }

        [NotMapped]
        public int EventId => Id;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public DateTime CreatedDate => CreatedAt;

        public int Priority { get; set; } = 1; // 1=High, 2=Medium, 3=Low

        public int ViewCount { get; set; } = 0;

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        public string? Organizer { get; set; }
    }
}
