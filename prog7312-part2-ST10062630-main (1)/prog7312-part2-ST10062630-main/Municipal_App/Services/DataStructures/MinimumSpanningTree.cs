using System;
using System.Collections.Generic;
using System.Linq;

namespace Municipal_App.Services.DataStructures
{
    /// <summary>
    /// Minimum Spanning Tree using Kruskal's algorithm
    /// Finds optimal connections between related service requests
    /// Used to identify clusters of related issues and optimize resource allocation
    /// </summary>
    public class MinimumSpanningTree
    {
        public class Edge : IComparable<Edge>
        {
            public int Source { get; set; }
            public int Destination { get; set; }
            public double Weight { get; set; }

            public int CompareTo(Edge? other)
            {
                if (other == null) return 1;
                return Weight.CompareTo(other.Weight);
            }
        }

        private class UnionFind
        {
            private Dictionary<int, int> parent = new Dictionary<int, int>();
            private Dictionary<int, int> rank = new Dictionary<int, int>();

            public void MakeSet(int node)
            {
                if (!parent.ContainsKey(node))
                {
                    parent[node] = node;
                    rank[node] = 0;
                }
            }

            public int Find(int node)
            {
                if (!parent.ContainsKey(node))
                    MakeSet(node);

                if (parent[node] != node)
                    parent[node] = Find(parent[node]);

                return parent[node];
            }

            public bool Union(int node1, int node2)
            {
                int root1 = Find(node1);
                int root2 = Find(node2);

                if (root1 == root2)
                    return false;

                if (rank[root1] < rank[root2])
                {
                    parent[root1] = root2;
                }
                else if (rank[root1] > rank[root2])
                {
                    parent[root2] = root1;
                }
                else
                {
                    parent[root2] = root1;
                    rank[root1]++;
                }

                return true;
            }
        }

        public List<Edge> FindMST(List<Edge> edges)
        {
            var mst = new List<Edge>();
            var uf = new UnionFind();
            var sortedEdges = edges.OrderBy(e => e.Weight).ToList();

            // Initialize union-find
            foreach (var edge in edges)
            {
                uf.MakeSet(edge.Source);
                uf.MakeSet(edge.Destination);
            }

            // Kruskal's algorithm
            foreach (var edge in sortedEdges)
            {
                if (uf.Union(edge.Source, edge.Destination))
                {
                    mst.Add(edge);
                }
            }

            return mst;
        }

        public double GetTotalWeight(List<Edge> mst)
        {
            return mst.Sum(e => e.Weight);
        }

        public Dictionary<int, List<int>> GetMSTAsAdjacencyList(List<Edge> mst)
        {
            var adjacencyList = new Dictionary<int, List<int>>();

            foreach (var edge in mst)
            {
                if (!adjacencyList.ContainsKey(edge.Source))
                    adjacencyList[edge.Source] = new List<int>();
                if (!adjacencyList.ContainsKey(edge.Destination))
                    adjacencyList[edge.Destination] = new List<int>();

                adjacencyList[edge.Source].Add(edge.Destination);
                adjacencyList[edge.Destination].Add(edge.Source);
            }

            return adjacencyList;
        }
    }
}
