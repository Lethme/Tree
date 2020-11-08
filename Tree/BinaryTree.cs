using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    #region Enums
    /// <summary>
    /// Tree pass type
    /// </summary>
    public enum PassType
    {
        /// <summary>
        /// Root - left leaf - right leaf
        /// </summary>
        PreOrder,
        /// <summary>
        /// Left leaf - right leaf - root
        /// </summary>
        PostOrder,
        /// <summary>
        /// Left leaf - root - right leaf
        /// </summary>
        HybridOrder,
        /// <summary>
        /// Order by tree floors (root is zero floor, his left and right leafs are first etc)
        /// </summary>
        FloorsOrder
    }
    /// <summary>
    /// Output string format
    /// </summary>
    public enum StringFormat
    {
        /// <summary>
        /// Line without any indents
        /// </summary>
        SingleLine,
        /// <summary>
        /// Line with indents
        /// </summary>
        Indented
    }
    #endregion
    #region Exceptions
    /// <summary>
    /// <para>Object duplication exception</para>
    /// <para>Appears while adding object which already exist in a tree</para>
    /// </summary>
    /// <typeparam name="T">As tree can contain a different types of objects</typeparam>
    [Serializable]
    public class ObjectDuplicationException<T> : Exception
    {
        public ObjectDuplicationException(T Object) : base ($"Object '{Object}' is already exist in current tree!") {  }
        public ObjectDuplicationException(string message) : base(message) {  }
        public ObjectDuplicationException(string message, Exception innerException) : base(message, innerException) {  }
        public ObjectDuplicationException() {  }
        protected ObjectDuplicationException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    /// <summary>
    /// Binary tree leaf class
    /// </summary>
    /// <typeparam name="T">Type of leaf value</typeparam>
    public class BinaryTreeNode<T> : IComparable<BinaryTreeNode<T>>, IEquatable<BinaryTreeNode<T>> where T : IComparable<T>
    {
        #region Properties
        /// <summary>
        /// Link to a parent leaf node
        /// </summary>
        public BinaryTreeNode<T> Parent { get; set; }
        /// <summary>
        /// Link to a left leaf node
        /// </summary>
        public BinaryTreeNode<T> Left { get; set; }
        /// <summary>
        /// Link to a right leaf node
        /// </summary>
        public BinaryTreeNode<T> Right { get; set; }
        /// <summary>
        /// Leaf value
        /// </summary>
        public T Value { get; set; }
        #endregion
        #region Constructors
        /// <summary>
        /// Tree node constructor
        /// </summary>
        /// <param name="Object">Leaf value to be set</param>
        public BinaryTreeNode(T Object)
        {
            if (Object == null) throw new NullReferenceException();
            Value = Object;
        }
        #endregion
        #region Service
        /// <summary>
        /// Leafs comparator
        /// </summary>
        /// <param name="obj">Another leaf</param>
        /// <returns>Difference between two leafs</returns>
        public int CompareTo(BinaryTreeNode<T> obj)
        {
            if (obj == null) throw new NullReferenceException();
            return Value.CompareTo(obj.Value);
        }
        /// <summary>
        /// Leafs equation
        /// </summary>
        /// <param name="OtherNode">Another leaf</param>
        /// <returns><c>true</c> if leafs values are the same and <c>false</c> otherwise</returns>
        public bool Equals(BinaryTreeNode<T> OtherNode)
        {
            if (OtherNode == null) throw new NullReferenceException();
            return Value.CompareTo(OtherNode.Value) == 0;
        }
        /// <summary>
        /// Leaf to string converter
        /// </summary>
        /// <returns>Leaf value as string value</returns>
        public override string ToString() => Value.ToString();
        /// <summary>
        /// Leaf hash converter
        /// </summary>
        /// <returns>Leaf value hash</returns>
        public override int GetHashCode() => Value.GetHashCode();
        #endregion
    }
    /// <summary>
    /// Binary tree class
    /// </summary>
    /// <typeparam name="T">Type of tree leafs values</typeparam>
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Properties
        /// <summary>
        /// Link to tree root
        /// </summary>
        private BinaryTreeNode<T> Root { get; set; }
        /// <summary>
        /// Sorted list of tree leafs values
        /// </summary>
        public List<T> SortedPassList => Pass(PassType.HybridOrder, default);
        /// <summary>
        /// List of tree leafs value by tree floors
        /// </summary>
        public List<List<T>> FloorList => FloorPass(Root).Select(list => list.Select(node => node.Value).ToList()).ToList();
        /// <summary>
        /// Total tree leafs count
        /// </summary>
        public int Count => SortedPassList.Count;
        /// <summary>
        /// Total tree height
        /// </summary>
        public int Height => FloorPass(Root).Count;
        /// <summary>
        /// Maximal value of tree leafs values
        /// </summary>
        public T MaxValue => SortedPassList.Aggregate((a, b) => a.CompareTo(b) > 0 ? a : b);
        /// <summary>
        /// Minimal value of tree leafs values
        /// </summary>
        public T MinValue => SortedPassList.Aggregate((a, b) => b.CompareTo(a) > 0 ? a : b);
        /// <summary>
        /// Tree indexer
        /// </summary>
        /// <param name="index">Leaf index</param>
        /// <returns>Leaf value</returns>
        public T this[int index]
        {
            get { return Pass(PassType.FloorsOrder)[index].Value; }
            set { Pass(PassType.FloorsOrder)[index].Value = value; }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Binary tree constructor
        /// </summary>
        /// <param name="ObjectSequence">Sequence of values</param>
        public BinaryTree(params T[] ObjectSequence) => Add(ObjectSequence);
        /// <summary>
        /// Binary tree constructor
        /// </summary>
        /// <param name="Collection">Any enumerable collection</param>
        public BinaryTree(IEnumerable<T> Collection) => Add(Collection);
        #endregion
        #region Editors
        /// <summary>
        /// Allows to add new leafs by sequence of values
        /// </summary>
        /// <param name="ObjectSequence">Sequence of values</param>
        public void Add(params T[] ObjectSequence)
        {
            foreach (var obj in ObjectSequence) Add(obj);
        }
        /// <summary>
        /// Allows to add new leafs by any enumerable collection
        /// </summary>
        /// <param name="Collection">Any enumerable collection</param>
        public void Add(IEnumerable<T> Collection)
        {
            if (Collection == null) throw new NullReferenceException();
            foreach (var obj in Collection) Add(obj);
        }
        /// <summary>
        /// Allows to add new leaf
        /// </summary>
        /// <param name="Object">Leaf value</param>
        private void Add(T Object)
        {
            if (Root == null)
            {
                Root = new BinaryTreeNode<T>(Object);
            }
            else
            {
                if (IsItemExist(Object)) throw new ObjectDuplicationException<T>(Object);
                Add(new BinaryTreeNode<T>(Object), Root);
            }
        }
        /// <summary>
        /// Private tree node add method
        /// </summary>
        /// <param name="TreeNode">New leaf to add</param>
        /// <param name="AddLink">
        /// <para>Link of basic leaf to add new one</para>
        /// <para>Adding algorithm should always start from Root</para>
        /// </param>
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
        /// <summary>
        /// Removes first found leaf
        /// </summary>
        /// <param name="Value">Value to search leafs</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>Total removes leafs count</returns>
        public int Remove(T Value, PassType passOrder = PassType.FloorsOrder)
        {
            var RemoveItem = FindNodeByValue(Value, passOrder);
            if (RemoveItem != null) { RemoveNode(RemoveItem, passOrder); return 1; }
            return 0;
        }
        /// <summary>
        /// Removes all found leafs
        /// </summary>
        /// <param name="Value">Value to search leafs</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>Total removes leafs count</returns>
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
        /// <summary>
        /// Removes node from tree
        /// </summary>
        /// <param name="TreeNode">Tree node</param>
        /// <param name="passType">Order of tree pass</param>
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
        /// <summary>
        /// Clear tree
        /// </summary>
        public void Clear() => Root = null;
        #endregion
        #region Search
        /// <summary>
        /// Find all nodes by their value
        /// </summary>
        /// <param name="Value">Value to search</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>List of tree nodes</returns>
        private List<BinaryTreeNode<T>> FindNodeListByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            return Pass(passOrder).Where(x => x.Value.CompareTo(Value) == 0).ToList();
        }
        /// <summary>
        /// Find first node by value
        /// </summary>
        /// <param name="Value">Value to search</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>Node link</returns>
        private BinaryTreeNode<T> FindNodeByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            var PassList = FindNodeListByValue(Value, passOrder);
            if (PassList.Count != 0) return PassList[0];
            return null;
        }
        /// <summary>
        /// Find node by its index in tree
        /// </summary>
        /// <param name="NodeIndex">Index</param>
        /// <returns>Node link</returns>
        private BinaryTreeNode<T> FindNodeByIndex(int NodeIndex)
        {
            var pass = Pass(Root, PassType.FloorsOrder);
            if (NodeIndex < pass.Count) return pass[NodeIndex];
            return null;
        }
        #endregion
        #region Pass
        /// <summary>
        /// Get a list of tree leaf values with a specified pass order
        /// </summary>
        /// <param name="passOrder">Pass order type</param>
        /// <param name="NodeIndex">Start leaf index</param>
        /// <returns>List of tree leaf values</returns>
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
        /// <summary>
        /// Privare pass method
        /// </summary>
        /// <param name="passOrder">Pass order type</param>
        /// <returns>List of tree leafs</returns>
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
        /// <summary>
        /// Private pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="passOrder">Pass order type</param>
        /// <returns>List of tree leafs</returns>
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
        /// <summary>
        /// Pass tree by its floors
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="FloorList">
        /// <para>Final list of tree floors</para>
        /// <para>Should be null on method call!</para>
        /// </param>
        /// <returns>List of tree leafs ordered by floors</returns>
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
        /// <summary>
        /// Post order type pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="PassList">
        /// <para>Final list of tree leafs</para>
        /// <para>Should be null on method call</para>
        /// </param>
        /// <returns>List of tree leafs</returns>
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
        /// <summary>
        /// Pre order type pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="PassList">
        /// <para>Final list of tree leafs</para>
        /// <para>Should be null on method call</para>
        /// </param>
        /// <returns>List of tree leafs</returns>
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
        /// <summary>
        /// Hybrid order type pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="PassList">
        /// <para>Final list of tree leafs</para>
        /// <para>Should be null on method call</para>
        /// </param>
        /// <returns>List of tree leafs</returns>
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
        #endregion
        #region Service
        /// <summary>
        /// Tree to string converter
        /// </summary>
        /// <param name="stringFormat">Format of output string</param>
        /// <returns>String representation of tree</returns>
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
        /// <summary>
        /// Object existing checker
        /// </summary>
        /// <param name="Object">Object of tree</param>
        /// <returns><c>true</c> if object exists in tree and <c>false</c> otherwise</returns>
        public bool IsItemExist(T Object) => SortedPassList.Contains(Object);
        /// <summary>
        /// Basic tree to string converter
        /// </summary>
        /// <returns>String representation of tree</returns>
        public override string ToString() => ToString(StringFormat.Indented);
        #endregion
        #region Interfaces
        public IEnumerator<T> GetEnumerator() => SortedPassList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => SortedPassList.GetEnumerator();
        #endregion
    }
}
