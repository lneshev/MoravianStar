using System.Collections.Generic;

namespace MoravianStar.Extensions
{
    public static class ICollectionExtensions
    {
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value)
            where T : class
        {
            if (collection != null && value != null)
            {
                collection.Add(value);
            }
        }

        public static void AddIfNotNull<T>(this ICollection<T> collection, T? value)
            where T : struct
        {
            if (collection != null && value.HasValue)
            {
                collection.Add(value.Value);
            }
        }
    }
}