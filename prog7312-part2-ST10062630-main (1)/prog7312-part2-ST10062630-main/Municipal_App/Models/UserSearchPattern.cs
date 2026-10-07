using System;
using System.ComponentModel.DataAnnotations;

namespace Municipal_App.Models
{
    public class UserSearchPattern
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string SearchTerm { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Category { get; set; }

        public int SearchCount { get; set; } = 1;

        public int ResultCount { get; set; } = 0;

        [Required]
        public DateTime SearchDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime LastSearched { get; set; } = DateTime.Now;

        public virtual User User { get; set; } = null!;
    }
}
