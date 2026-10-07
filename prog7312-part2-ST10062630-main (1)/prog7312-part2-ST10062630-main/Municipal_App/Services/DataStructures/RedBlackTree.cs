using System;
using System.Collections.Generic;

namespace Municipal_App.Services.DataStructures
{
    /// <summary>
    /// Red-Black Tree implementation for balanced service request storage
    /// Guarantees O(log n) operations with simpler balancing than AVL
    /// Used for maintaining sorted order while allowing frequent insertions
    /// </summary>
    public class RedBlackTree<T> where T : class
    {
        public enum Color { Red, Black }

        public class RBNode
        {
            public int Key { get; set; }
            public T Data { get; set; }
            public RBNode? Left { get; set; }
            public RBNode? Right { get; set; }
            public RBNode? Parent { get; set; }
            public Color NodeColor { get; set; }

            public RBNode(int key, T data)
            {
                Key = key;
                Data = data;
                NodeColor = Color.Red;
            }
        }

        public RBNode? Root { get; private set; }

        public void Insert(int key, T data)
        {
            RBNode newNode = new RBNode(key, data);
            Root = BSTInsert(Root, newNode);
            FixViolation(newNode);
        }

        private RBNode? BSTInsert(RBNode? root, RBNode newNode)
        {
            if (root == null)
                return newNode;

            if (newNode.Key < root.Key)
            {
                root.Left = BSTInsert(root.Left, newNode);
                if (root.Left != null)
                    root.Left.Parent = root;
            }
            else if (newNode.Key > root.Key)
            {
                root.Right = BSTInsert(root.Right, newNode);
                if (root.Right != null)
                    root.Right.Parent = root;
            }

            return root;
        }

        private void FixViolation(RBNode node)
        {
            RBNode? parent = null;
            RBNode? grandParent = null;

            while (node != Root && node.NodeColor == Color.Red && node.Parent?.NodeColor == Color.Red)
            {
                parent = node.Parent;
                grandParent = parent?.Parent;

                if (grandParent == null) break;

                if (parent == grandParent.Left)
                {
                    RBNode? uncle = grandParent.Right;

                    if (uncle != null && uncle.NodeColor == Color.Red)
                    {
                        grandParent.NodeColor = Color.Red;
                        parent.NodeColor = Color.Black;
                        uncle.NodeColor = Color.Black;
                        node = grandParent;
                    }
                    else
                    {
                        if (node == parent.Right)
                        {
                            RotateLeft(parent);
                            node = parent;
                            parent = node.Parent;
                        }

                        if (parent != null)
                        {
                            RotateRight(grandParent);
                            Color temp = parent.NodeColor;
                            parent.NodeColor = grandParent.NodeColor;
                            grandParent.NodeColor = temp;
                            node = parent;
                        }
                    }
                }
                else
                {
                    RBNode? uncle = grandParent.Left;

                    if (uncle != null && uncle.NodeColor == Color.Red)
                    {
                        grandParent.NodeColor = Color.Red;
                        parent.NodeColor = Color.Black;
                        uncle.NodeColor = Color.Black;
                        node = grandParent;
                    }
                    else
                    {
                        if (node == parent.Left)
                        {
                            RotateRight(parent);
                            node = parent;
                            parent = node.Parent;
                        }

                        if (parent != null)
                        {
                            RotateLeft(grandParent);
                            Color temp = parent.NodeColor;
                            parent.NodeColor = grandParent.NodeColor;
                            grandParent.NodeColor = temp;
                            node = parent;
                        }
                    }
                }
            }

            if (Root != null)
                Root.NodeColor = Color.Black;
        }

        private void RotateLeft(RBNode node)
        {
            RBNode? rightChild = node.Right;
            node.Right = rightChild?.Left;

            if (node.Right != null)
                node.Right.Parent = node;

            if (rightChild != null)
                rightChild.Parent = node.Parent;

            if (node.Parent == null)
                Root = rightChild;
            else if (node == node.Parent.Left)
                node.Parent.Left = rightChild;
            else
                node.Parent.Right = rightChild;

            if (rightChild != null)
                rightChild.Left = node;
            node.Parent = rightChild;
        }

        private void RotateRight(RBNode node)
        {
            RBNode? leftChild = node.Left;
            node.Left = leftChild?.Right;

            if (node.Left != null)
                node.Left.Parent = node;

            if (leftChild != null)
                leftChild.Parent = node.Parent;

            if (node.Parent == null)
                Root = leftChild;
            else if (node == node.Parent.Right)
                node.Parent.Right = leftChild;
            else
                node.Parent.Left = leftChild;

            if (leftChild != null)
                leftChild.Right = node;
            node.Parent = leftChild;
        }

        public T? Search(int key)
        {
            return SearchRec(Root, key);
        }

        private T? SearchRec(RBNode? node, int key)
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

        private void InOrderRec(RBNode? node, List<T> result)
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
