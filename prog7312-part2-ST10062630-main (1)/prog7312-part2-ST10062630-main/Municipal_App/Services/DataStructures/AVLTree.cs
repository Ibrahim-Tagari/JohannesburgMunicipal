using System;
using System.Collections.Generic;

namespace Municipal_App.Services.DataStructures
{
    /// <summary>
    /// Self-balancing AVL Tree for guaranteed O(log n) operations
    /// Maintains balance factor to ensure tree height is minimal
    /// Used for fast service request lookups by priority
    /// </summary>
    public class AVLTree<T> where T : class
    {
        public class AVLNode
        {
            public int Key { get; set; }
            public T Data { get; set; }
            public AVLNode? Left { get; set; }
            public AVLNode? Right { get; set; }
            public int Height { get; set; }

            public AVLNode(int key, T data)
            {
                Key = key;
                Data = data;
                Height = 1;
            }
        }

        public AVLNode? Root { get; private set; }

        private int Height(AVLNode? node)
        {
            return node?.Height ?? 0;
        }

        private int GetBalance(AVLNode? node)
        {
            return node == null ? 0 : Height(node.Left) - Height(node.Right);
        }

        private AVLNode RightRotate(AVLNode y)
        {
            AVLNode x = y.Left!;
            AVLNode? T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            return x;
        }

        private AVLNode LeftRotate(AVLNode x)
        {
            AVLNode y = x.Right!;
            AVLNode? T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

            return y;
        }

        public void Insert(int key, T data)
        {
            Root = InsertRec(Root, key, data);
        }

        private AVLNode InsertRec(AVLNode? node, int key, T data)
        {
            if (node == null)
                return new AVLNode(key, data);

            if (key < node.Key)
                node.Left = InsertRec(node.Left, key, data);
            else if (key > node.Key)
                node.Right = InsertRec(node.Right, key, data);
            else
                return node;

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);

            // Left Left Case
            if (balance > 1 && key < node.Left!.Key)
                return RightRotate(node);

            // Right Right Case
            if (balance < -1 && key > node.Right!.Key)
                return LeftRotate(node);

            // Left Right Case
            if (balance > 1 && key > node.Left!.Key)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left Case
            if (balance < -1 && key < node.Right!.Key)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        public T? Search(int key)
        {
            return SearchRec(Root, key);
        }

        private T? SearchRec(AVLNode? node, int key)
        {
            if (node == null || node.Key == key)
                return node?.Data;

            if (key < node.Key)
                return SearchRec(node.Left, key);

            return SearchRec(node.Right, key);
        }

        public List<T> InOrderTraversal()
        {
            var result = new List<T>();
            InOrderRec(Root, result);
            return result;
        }

        private void InOrderRec(AVLNode? node, List<T> result)
        {
            if (node != null)
            {
                InOrderRec(node.Left, result);
                result.Add(node.Data);
                InOrderRec(node.Right, result);
            }
        }
    }
}
