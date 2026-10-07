using System.Collections.Generic;

namespace Municipal_App.Models
{
    /// <summary>
    /// Graph edge representing relationships between service requests
    /// Used for dependency tracking and MST calculations
    /// </summary>
    public class ServiceRequestEdge
    {
        public int SourceId { get; set; }
        public int DestinationId { get; set; }
        public double Weight { get; set; }
        public string RelationType { get; set; } = "Related";

        public ServiceRequestEdge(int source, int destination, double weight, string relationType = "Related")
        {
            SourceId = source;
            DestinationId = destination;
            Weight = weight;
            RelationType = relationType;
        }
    }

    /// <summary>
    /// Graph node representing a service request in the dependency graph
    /// </summary>
    public class ServiceRequestNode
    {
        public int Id { get; set; }
        public string Status { get; set; } = "Submitted";
        public int Priority { get; set; }
        public List<ServiceRequestEdge> Edges { get; set; } = new List<ServiceRequestEdge>();

        public ServiceRequestNode(int id, string status, int priority)
        {
            Id = id;
            Status = status;
            Priority = priority;
        }
    }
}
