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
            foreach (var floor in Tree.FloorList)
            {
                foreach (var item in floor) Console.Write($"{item} ");
                Console.WriteLine();
            }
        }
    }
}
