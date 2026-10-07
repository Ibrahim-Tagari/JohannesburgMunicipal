using System;
using System.Collections.Generic;
using Municipal_App.Models;

namespace Municipal_App.Services.DataStructures
{
    /// <summary>
    /// Binary Search Tree implementation for organizing service requests by priority
    /// Provides O(log n) average-case search, insert, and delete operations
    /// </summary>
    public class BinarySearchTree<T> where T : class
    {
        public class BSTNode
        {
            public int Key { get; set; }
            public T Data { get; set; }
            public BSTNode? Left { get; set; }
            public BSTNode? Right { get; set; }

            public BSTNode(int key, T data)
            {
                Key = key;
                Data = data;
                Left = null;
                Right = null;
            }
        }

        public BSTNode? Root { get; private set; }

        public void Insert(int key, T data)
        {
            Root = InsertRec(Root, key, data);
        }

        private BSTNode InsertRec(BSTNode? root, int key, T data)
        {
            if (root == null)
            {
                return new BSTNode(key, data);
            }

            if (key < root.Key)
                root.Left = InsertRec(root.Left, key, data);
            else if (key > root.Key)
                root.Right = InsertRec(root.Right, key, data);

            return root;
        }

        public T? Search(int key)
        {
            return SearchRec(Root, key);
        }

        private T? SearchRec(BSTNode? root, int key)
        {
            if (root == null || root.Key == key)
                return root?.Data;

            if (key < root.Key)
                return SearchRec(root.Left, key);

            return SearchRec(root.Right, key);
        }

        public List<T> InOrderTraversal()
        {
            var result = new List<T>();
            InOrderRec(Root, result);
            return result;
        }

        private void InOrderRec(BSTNode? root, List<T> result)
        {
            if (root != null)
            {
                InOrderRec(root.Left, result);
                result.Add(root.Data);
                InOrderRec(root.Right, result);
            }
        }

        public List<T> GetRange(int minKey, int maxKey)
        {
            var result = new List<T>();
            GetRangeRec(Root, minKey, maxKey, result);
            return result;
        }

        private void GetRangeRec(BSTNode? root, int minKey, int maxKey, List<T> result)
        {
            if (root == null) return;

            if (root.Key > minKey)
                GetRangeRec(root.Left, minKey, maxKey, result);

            if (root.Key >= minKey && root.Key <= maxKey)
                result.Add(root.Data);

            if (root.Key < maxKey)
                GetRangeRec(root.Right, minKey, maxKey, result);
        }
    }
}
