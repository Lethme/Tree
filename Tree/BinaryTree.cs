using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public enum PassType
    {
        PreOrder,
        PostOrder,
        HybridOrder
    }
    public enum DeletionType
    {
        DeleteAll,
        FirstOnly
    }
    public class BinaryTreeNode<T> : IComparable<BinaryTreeNode<T>>, IEquatable<BinaryTreeNode<T>> where T : IComparable<T>
    {
        public BinaryTreeNode<T> Parent { get; set; }
        public BinaryTreeNode<T> Left { get; set; }
        public BinaryTreeNode<T> Right { get; set; }
        public T Value { get; set; }
        public BinaryTreeNode(T Object)
        {
            if (Object == null) throw new NullReferenceException();
            Value = Object;
        }
        public int CompareTo(BinaryTreeNode<T> obj)
        {
            if (obj == null) throw new NullReferenceException();
            return Value.CompareTo(obj.Value);
        }
        public bool Equals(BinaryTreeNode<T> OtherNode)
        {
            if (OtherNode == null) throw new NullReferenceException();
            return Value.CompareTo(OtherNode.Value) == 0;
        }
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
    }
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        public BinaryTreeNode<T> Root { get; private set; }
        public List<T> SortedPassList => Pass(PassType.HybridOrder).Select((node) => node.Value).ToList();
        public T MaxValue => Utils.FindMaxValue(SortedPassList);
        public T MinValue => Utils.FindMinValue(SortedPassList);
        public BinaryTree(T Object) => Add(Object);
        public BinaryTree(params T[] ObjectSequence) => Add(ObjectSequence);
        public BinaryTree(IEnumerable<T> Collection) => Add(Collection);
        public void Add(T Object) 
        {
            if (Root == null)
            {
                Root = new BinaryTreeNode<T>(Object);
            }
            else
            {
                Add(new BinaryTreeNode<T>(Object), Root);
            }
        }
        public void Add(params T[] ObjectSequence)
        {
            if (ObjectSequence.Length == 0) throw new NullReferenceException();
            foreach (var obj in ObjectSequence) Add(obj);
        }
        public void Add(IEnumerable<T> Collection)
        {
            if (Collection == null) throw new NullReferenceException();
            foreach (var obj in Collection) Add(obj);
        }
        private void Add(BinaryTreeNode<T> TreeNode, BinaryTreeNode<T> AddLink)
        {
            if (TreeNode == null) throw new NullReferenceException();
            if (TreeNode.CompareTo(AddLink) > 0)
            {
                if (AddLink.Right == null)
                {
                    AddLink.Right = TreeNode;
                    TreeNode.Parent = AddLink;
                }
                else
                {
                    Add(TreeNode, AddLink.Right);
                }
            }
            else if (TreeNode.CompareTo(AddLink) <= 0)
            {
                if (AddLink.Left == null)
                {
                    AddLink.Left = TreeNode;
                    TreeNode.Parent = AddLink;
                }
                else
                {
                    Add(TreeNode, AddLink.Left);
                }
            }
        }
        public void RemoveNode(T Value, DeletionType deletionType = DeletionType.DeleteAll, PassType passOrder = PassType.HybridOrder)
        {
            switch (deletionType)
            {
                case DeletionType.DeleteAll:
                    {
                        BinaryTreeNode<T> RemoveItem;
                        while ((RemoveItem = FindNodeByValue(Value, passOrder)) != null)
                        {
                            RemoveNode(RemoveItem);
                        }
                        break;
                    }
                case DeletionType.FirstOnly:
                    {
                        var RemoveItem = FindNodeByValue(Value, passOrder);
                        if (RemoveItem != null) RemoveNode(RemoveItem);
                        break;
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        private void RemoveNode(BinaryTreeNode<T> TreeNode)
        {
            if ((TreeNode.Left ?? TreeNode.Right) == null)
            {
                if (TreeNode == TreeNode.Parent.Left) TreeNode.Parent.Left = null;
                if (TreeNode == TreeNode.Parent.Right) TreeNode.Parent.Right = null;
            }
            else
            {
                var RemovedItemsList = HybridOrderPass(TreeNode).Where(x => x != TreeNode).Select(x => x.Value).ToList();
                if (TreeNode == Root)
                {
                    Root = null;
                }
                else
                {
                    if (TreeNode == TreeNode.Parent.Left) TreeNode.Parent.Left = null;
                    if (TreeNode == TreeNode.Parent.Right) TreeNode.Parent.Right = null;
                }
                foreach (var item in RemovedItemsList) Add(item);
            }
        }
        public List<BinaryTreeNode<T>> FindNodeListByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            return Pass(passOrder).Where(x => x.Value.CompareTo(Value) == 0).ToList();
        }
        public BinaryTreeNode<T> FindNodeByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            var PassList = Pass(passOrder).Where(x => x.Value.CompareTo(Value) == 0).ToList();
            if (PassList.Count != 0) return PassList[0];
            return null;
        }
        public T GetMaxValue()
        {
            var MaxValue = this.MaxValue;
            var PassList = Pass(PassType.PostOrder).Where((x) => x.Value.CompareTo(MaxValue) == 0).ToList();
            for (int i = 1; i < PassList.Count; i++) PassList[i].Value = default;
            return MaxValue;
        }
        public T GetMinValue()
        {
            var MinValue = this.MinValue;
            var PassList = Pass(PassType.PostOrder).Where((x) => x.Value.CompareTo(MinValue) == 0).ToList();
            for (int i = 1; i < PassList.Count; i++) PassList[i].Value = default;
            return MinValue;
        }
        public List<BinaryTreeNode<T>> Pass(PassType passOrder = PassType.PreOrder)
        {
            switch (passOrder)
            {
                case PassType.PreOrder: { return PreOrderPass(Root); }
                case PassType.PostOrder: { return PostOrderPass(Root); }
                case PassType.HybridOrder: { return HybridOrderPass(Root); }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        private List<BinaryTreeNode<T>> PostOrderPass(BinaryTreeNode<T> Node, List<BinaryTreeNode<T>> PassList = null)
        {
            if (PassList == null) PassList = new List<BinaryTreeNode<T>>();
            if (Node != null)
            {
                PostOrderPass(Node.Left, PassList);
                PostOrderPass(Node.Right, PassList);
                PassList.Add(Node);
            }
            return PassList;
        }
        private List<BinaryTreeNode<T>> PreOrderPass(BinaryTreeNode<T> Node, List<BinaryTreeNode<T>> PassList = null)
        {
            if (PassList == null) PassList = new List<BinaryTreeNode<T>>();
            if (Node != null)
            {
                PassList.Add(Node);
                PreOrderPass(Node.Left, PassList);
                PreOrderPass(Node.Right, PassList);
            }
            return PassList;
        }
        private List<BinaryTreeNode<T>> HybridOrderPass(BinaryTreeNode<T> Node, List<BinaryTreeNode<T>> PassList = null)
        {
            if (PassList == null) PassList = new List<BinaryTreeNode<T>>();
            if (Node != null)
            {
                HybridOrderPass(Node.Left, PassList);
                PassList.Add(Node);
                HybridOrderPass(Node.Right, PassList);
            }
            return PassList;
        }
        public override string ToString() => Root.ToString();

        public IEnumerator<T> GetEnumerator() => SortedPassList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => SortedPassList.GetEnumerator();
    }
}
