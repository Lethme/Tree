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
            //var Tree = new BinaryTree<int>(6, 5, 3, 8, 1, 4, 9, 15, 17, 13, 7);
            //foreach (var item in Tree.Pass(PassType.PostOrder)) Console.Write($"{item} ");
            //Console.WriteLine();
            var Tree = new BinaryTree<int>(7, 9, 5, 9, 9, 6);
            Console.WriteLine($"{Tree}\n");
            Console.WriteLine($"{Tree.GetMaxValue()}\n");
            Console.WriteLine($"{Tree}\n");
        }
    }
}
