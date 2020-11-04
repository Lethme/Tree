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
        HybridOrder,
        FloorsOrder
    }
    public enum StringFormat
    {
        SingleLine,
        Indented
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
        private BinaryTreeNode<T> Root { get; set; }
        public List<T> SortedPassList => Pass(PassType.HybridOrder).Select((node) => node.Value).ToList();
        public List<List<T>> FloorList => FloorPass(Root).Select(list => list.Select(node => node.Value).ToList()).ToList();
        public int Count => SortedPassList.Count;
        public int Height => FloorPass(Root).Count;
        public T MaxValue => SortedPassList.Aggregate((a, b) => a.CompareTo(b) > 0 ? a : b);
        public T MinValue => SortedPassList.Aggregate((a, b) => b.CompareTo(a) > 0 ? a : b);
        public BinaryTree(T Object) => Add(Object);
        public BinaryTree(params T[] ObjectSequence) => Add(ObjectSequence);
        public BinaryTree(IEnumerable<T> Collection) => Add(Collection);
        public T this[int index]
        {
            get { return Pass(PassType.FloorsOrder)[index].Value; }
            set { Pass(PassType.FloorsOrder)[index].Value = value; }
        }
        public bool IsItemExist(T Object) => SortedPassList.Contains(Object);
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
        public int Remove(T Value, PassType passOrder = PassType.FloorsOrder)
        {
            var RemoveItem = FindNodeByValue(Value, passOrder);
            if (RemoveItem != null) { RemoveNode(RemoveItem, passOrder); return 1; }
            return 0;
        }
        public int RemoveAll(T Value, PassType passOrder = PassType.FloorsOrder)
        {
            BinaryTreeNode<T> RemoveItem;
            int RemoveCount = 0;
            while ((RemoveItem = FindNodeByValue(Value, passOrder)) != null)
            {
                RemoveCount++;
                RemoveNode(RemoveItem, passOrder);
            }
            return RemoveCount;
        }
        private void RemoveNode(BinaryTreeNode<T> TreeNode, PassType passType = PassType.FloorsOrder)
        {
            if ((TreeNode.Left ?? TreeNode.Right) == null && TreeNode != Root)
            {
                if (TreeNode == TreeNode.Parent.Left) TreeNode.Parent.Left = null;
                if (TreeNode == TreeNode.Parent.Right) TreeNode.Parent.Right = null;
            }
            else
            {
                var RemovedItemsList = Pass(TreeNode, passType).Where(x => x != TreeNode).Select(x => x.Value).ToList();
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
        public void Clear() => Root = null;
        private List<BinaryTreeNode<T>> FindNodeListByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            return Pass(passOrder).Where(x => x.Value.CompareTo(Value) == 0).ToList();
        }
        private BinaryTreeNode<T> FindNodeByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            var PassList = Pass(passOrder).Where(x => x.Value.CompareTo(Value) == 0).ToList();
            if (PassList.Count != 0) return PassList[0];
            return null;
        }
        private BinaryTreeNode<T> FindNodeByIndex(int NodeIndex)
        {
            var pass = Pass(Root, PassType.FloorsOrder);
            if (NodeIndex < pass.Count) return pass[NodeIndex];
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
        public List<T> Pass(PassType passOrder = PassType.PreOrder, int NodeIndex = 0)
        {
            if (Root == null) return new List<T>();
            var Node = FindNodeByIndex(NodeIndex);
            switch (passOrder)
            {
                case PassType.PreOrder: { return PreOrderPass(Node).Select(x => x.Value).ToList(); }
                case PassType.PostOrder: { return PostOrderPass(Node).Select(x => x.Value).ToList(); }
                case PassType.HybridOrder: { return HybridOrderPass(Node).Select(x => x.Value).ToList(); }
                case PassType.FloorsOrder: 
                    {
                        var floorList = FloorPass(Node);
                        if (floorList.Count != 0) return FloorPass(Node).Aggregate((x, y) => x.Concat(y).ToList()).Select(x => x.Value).ToList();
                        else return new List<T>();
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        private List<BinaryTreeNode<T>> Pass(PassType passOrder = PassType.PreOrder)
        {
            switch (passOrder)
            {
                case PassType.PreOrder: { return PreOrderPass(Root); }
                case PassType.PostOrder: { return PostOrderPass(Root); }
                case PassType.HybridOrder: { return HybridOrderPass(Root); }
                case PassType.FloorsOrder: 
                    {
                        var floorList = FloorPass(Root);
                        if (floorList.Count != 0) return FloorPass(Root).Aggregate((x, y) => x.Concat(y).ToList());
                        else return new List<BinaryTreeNode<T>>();
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        private List<BinaryTreeNode<T>> Pass(BinaryTreeNode<T> Node, PassType passOrder = PassType.PreOrder)
        {
            switch (passOrder)
            {
                case PassType.PreOrder: { return PreOrderPass(Node); }
                case PassType.PostOrder: { return PostOrderPass(Node); }
                case PassType.HybridOrder: { return HybridOrderPass(Node); }
                case PassType.FloorsOrder:
                    {
                        var floorList = FloorPass(Node);
                        if (floorList.Count != 0) return FloorPass(Node).Aggregate((x, y) => x.Concat(y).ToList());
                        else return new List<BinaryTreeNode<T>>();
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        private List<List<BinaryTreeNode<T>>> FloorPass(BinaryTreeNode<T> Node, List<List<BinaryTreeNode<T>>> FloorList = null)
        {
            if (FloorList == null)
            {
                FloorList = new List<List<BinaryTreeNode<T>>>();
                if (Root == null) return FloorList;
                FloorList.Add(new List<BinaryTreeNode<T>>() { Node });
            }
            var CurrentFloor = new List<BinaryTreeNode<T>>();
            foreach (var item in FloorList.Last())
            {
                if (item.Left != null) CurrentFloor.Add(item.Left);
                if (item.Right != null) CurrentFloor.Add(item.Right);
            }
            FloorList.Add(CurrentFloor);
            if (FloorList.Last().Any(x => x != null)) FloorPass(null, FloorList);
            else FloorList.Remove(FloorList.Last());

            return FloorList;
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
        public string ToString(StringFormat stringFormat = StringFormat.SingleLine)
        {
            var tempStr = String.Empty;
            var tempList = FloorPass(Root);
            for (var i = tempList.Count - 1; i >= 0; i--)
            {
                for (var j = tempList[i].Count - 1; j >= 0; j--)
                {
                    if (i == 0) tempStr = tempStr.Insert(0, $"{tempList[i][j].Value}:Root ");
                    else
                    {
                        if (tempList[i][j].Parent.Left == tempList[i][j]) tempStr = tempStr.Insert(0, $"{tempList[i][j].Value}:{tempList[i][j].Parent.Value}L ");
                        if (tempList[i][j].Parent.Right == tempList[i][j]) tempStr = tempStr.Insert(0, $"{tempList[i][j].Value}:{tempList[i][j].Parent.Value}R ");
                    }
                }
                if (stringFormat == StringFormat.Indented && i != 0) tempStr = tempStr.Insert(0, Environment.NewLine);
            }
            return tempStr.Trim(' ');
        }
        public override string ToString() => ToString(StringFormat.Indented);

        public IEnumerator<T> GetEnumerator() => SortedPassList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => SortedPassList.GetEnumerator();
    }
}
