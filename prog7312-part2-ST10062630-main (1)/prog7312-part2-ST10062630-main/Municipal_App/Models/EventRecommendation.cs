using System.Collections.Generic;

namespace Municipal_App.Models
{
    public class EventRecommendation
    {
        public LocalEvent Event { get; set; } = null!;
        public double RelevanceScore { get; set; }
        public string RecommendationReason { get; set; } = string.Empty;
        public List<string> MatchedCategories { get; set; } = new List<string>();
    }
}
