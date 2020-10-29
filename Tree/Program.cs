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
            foreach (var item in Tree) Console.Write($"{item}\n");
        }
    }
}
