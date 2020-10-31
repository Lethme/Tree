using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    class Program
    {
        static void Main(string[] args)
        {
            var Tree = new BinaryTree<int>(6, 5, 3, 8, 1, 4, 9, 15, 17, 13, 7);

            foreach (var item in Tree.SortedPassList) Console.WriteLine(item);
            Console.WriteLine();
            Console.WriteLine($"{Tree.Count}\n{Tree.Height}\n");
            Tree.RemoveNode(9, DeletionType.DeleteAll, PassType.FloorsOrder);
            foreach (var item in Tree.SortedPassList) Console.WriteLine(item);
            Console.WriteLine();
            Console.WriteLine($"{Tree.Count}\n{Tree.Height}\n");
        }
    }
}
