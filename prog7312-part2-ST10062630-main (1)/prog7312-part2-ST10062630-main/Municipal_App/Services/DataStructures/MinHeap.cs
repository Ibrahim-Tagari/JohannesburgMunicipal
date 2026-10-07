using System;
using System.Collections.Generic;

namespace Municipal_App.Services.DataStructures
{
    /// <summary>
    /// Min-Heap implementation for priority queue operations
    /// Provides O(log n) insert and O(1) access to highest priority request
    /// Used to efficiently retrieve most urgent service requests
    /// </summary>
    public class MinHeap<T> where T : class
    {
        private class HeapNode
        {
            public int Priority { get; set; }
            public T Data { get; set; }

            public HeapNode(int priority, T data)
            {
                Priority = priority;
                Data = data;
            }
        }

        private List<HeapNode> heap = new List<HeapNode>();

        public int Count => heap.Count;

        public void Insert(int priority, T data)
        {
            heap.Add(new HeapNode(priority, data));
            HeapifyUp(heap.Count - 1);
        }

        public T? ExtractMin()
        {
            if (heap.Count == 0)
                return null;

            T minData = heap[0].Data;
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
                HeapifyDown(0);

            return minData;
        }

        public T? PeekMin()
        {
            return heap.Count > 0 ? heap[0].Data : null;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[index].Priority >= heap[parentIndex].Priority)
                    break;

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int smallest = index;
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;

                if (leftChild < heap.Count && heap[leftChild].Priority < heap[smallest].Priority)
                    smallest = leftChild;

                if (rightChild < heap.Count && heap[rightChild].Priority < heap[smallest].Priority)
                    smallest = rightChild;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            var temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        public List<T> GetAllElements()
        {
            var result = new List<T>();
            foreach (var node in heap)
            {
                result.Add(node.Data);
            }
            return result;
        }
    }
}
