using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public static class Utils
    {
        /// <summary>
        /// Represents equation checking method
        /// </summary>
        /// <typeparam name="T">Any comparable type</typeparam>
        /// <param name="firstObject">First object</param>
        /// <param name="secondObject">Second object</param>
        /// <returns>Return <c>true</c> if objects are equal and <c>false</c> otherwise</returns>
        public static bool Equals<T>(T firstObject, T secondObject) where T : IComparable<T>
        {
            return firstObject.CompareTo(secondObject) == 0;
        }
        /// <summary>
        /// Find mimimal value in stated sequence
        /// </summary>
        /// <typeparam name="T">Any comparable type</typeparam>
        /// <param name="Collection">Sequence of values</param>
        /// <returns>Minimal value</returns>
        public static T FindMinValue<T>(params T[] Collection) where T : IComparable<T>
        {
            return Collection.Aggregate((a, b) => b.CompareTo(a) > 0 ? a : b);
        }
        /// <summary>
        /// Find mimimal value in collection
        /// </summary>
        /// <typeparam name="T">Any comparable type</typeparam>
        /// <param name="Collection">Collection</param>
        /// <returns>Minimal value</returns>
        public static T FindMinValue<T>(IEnumerable<T> Collection) where T : IComparable<T>
        {
            return Collection.Aggregate((a, b) => b.CompareTo(a) > 0 ? a : b);
        }
        /// <summary>
        /// Find maximal value in stated sequence
        /// </summary>
        /// <typeparam name="T">Any comparable type</typeparam>
        /// <param name="Collection">Sequence of values</param>
        /// <returns>Maximal value</returns>
        public static T FindMaxValue<T>(params T[] Collection) where T : IComparable<T>
        {
            return Collection.Aggregate((a, b) => a.CompareTo(b) > 0 ? a : b);
        }
        /// <summary>
        /// Find maximal value in collection
        /// </summary>
        /// <typeparam name="T">Any comparable type</typeparam>
        /// <param name="Collection">Collection</param>
        /// <returns>Maximal value</returns>
        public static T FindMaxValue<T>(IEnumerable<T> Collection) where T : IComparable<T>
        {
            return Collection.Aggregate((a, b) => a.CompareTo(b) > 0 ? a : b);
        }
    }
}
