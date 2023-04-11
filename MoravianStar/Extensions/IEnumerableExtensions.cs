using System.Collections.Generic;
using System.Linq;

namespace MoravianStar.Extensions
{
    public static class IEnumerableExtensions
    {
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