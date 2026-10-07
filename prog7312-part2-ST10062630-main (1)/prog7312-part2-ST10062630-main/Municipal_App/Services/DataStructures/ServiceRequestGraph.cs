using System;
using System.Collections.Generic;
using System.Linq;
using Municipal_App.Models;

namespace Municipal_App.Services.DataStructures
{
    /// <summary>
    /// Graph implementation for service request dependencies and relationships
    /// Supports BFS, DFS traversal and identifies dependency chains
    /// Used to detect blocking issues and optimize processing order
    /// </summary>
    public class ServiceRequestGraph
    {
        private Dictionary<int, List<int>> adjacencyList = new Dictionary<int, List<int>>();
        private Dictionary<int, Issue> nodeData = new Dictionary<int, Issue>();

        public void AddNode(int nodeId, Issue issue)
        {
            if (!adjacencyList.ContainsKey(nodeId))
            {
                adjacencyList[nodeId] = new List<int>();
                nodeData[nodeId] = issue;
            }
        }

        public void AddEdge(int from, int to)
        {
            if (!adjacencyList.ContainsKey(from))
                adjacencyList[from] = new List<int>();

            if (!adjacencyList[from].Contains(to))
                adjacencyList[from].Add(to);
        }

        public List<Issue> BreadthFirstSearch(int startNode)
        {
            var result = new List<Issue>();
            var visited = new HashSet<int>();
            var queue = new Queue<int>();

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                if (nodeData.ContainsKey(current))
                    result.Add(nodeData[current]);

                if (adjacencyList.ContainsKey(current))
                {
                    foreach (int neighbor in adjacencyList[current])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return result;
        }

        public List<Issue> DepthFirstSearch(int startNode)
        {
            var result = new List<Issue>();
            var visited = new HashSet<int>();
            DFSRecursive(startNode, visited, result);
            return result;
        }

        private void DFSRecursive(int node, HashSet<int> visited, List<Issue> result)
        {
            visited.Add(node);
            if (nodeData.ContainsKey(node))
                result.Add(nodeData[node]);

            if (adjacencyList.ContainsKey(node))
            {
                foreach (int neighbor in adjacencyList[node])
                {
                    if (!visited.Contains(neighbor))
                        DFSRecursive(neighbor, visited, result);
                }
            }
        }

        public List<int> GetDependencies(int nodeId)
        {
            return adjacencyList.ContainsKey(nodeId) ? adjacencyList[nodeId] : new List<int>();
        }

        public bool HasCycle()
        {
            var visited = new HashSet<int>();
            var recursionStack = new HashSet<int>();

            foreach (var node in adjacencyList.Keys)
            {
                if (!visited.Contains(node))
                {
                    if (HasCycleDFS(node, visited, recursionStack))
                        return true;
                }
            }

            return false;
        }

        private bool HasCycleDFS(int node, HashSet<int> visited, HashSet<int> recursionStack)
        {
            visited.Add(node);
            recursionStack.Add(node);

            if (adjacencyList.ContainsKey(node))
            {
                foreach (int neighbor in adjacencyList[node])
                {
                    if (!visited.Contains(neighbor))
                    {
                        if (HasCycleDFS(neighbor, visited, recursionStack))
                            return true;
                    }
                    else if (recursionStack.Contains(neighbor))
                    {
                        return true;
                    }
                }
            }

            recursionStack.Remove(node);
            return false;
        }

        public Dictionary<int, List<int>> GetAdjacencyList()
        {
            return new Dictionary<int, List<int>>(adjacencyList);
        }
    }
}
