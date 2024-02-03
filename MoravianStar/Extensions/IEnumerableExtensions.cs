using System.Collections.Generic;
using System.Linq;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with IEnumerable objects.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Compares two <see cref="IEnumerable{T}"/> for equality by their length and element by element.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the arrays.</typeparam>
        /// <param name="array1">The source array.</param>
        /// <param name="array2">The target array.</param>
        /// <returns><see langword="true"/> if both arrays are null or have same number of elements and all elements are the same.</returns>
        public static bool ItemsEqual<T>(this IEnumerable<T> array1, IEnumerable<T> array2)
        {
            if (array1 == null && array2 == null)
            {
                return true;
            }

            if (array1 == null || array2 == null)
            {
                return false;
            }

            return array1.Count() == array2.Count() && !array1.Except(array2).Any();
        }
    }
}