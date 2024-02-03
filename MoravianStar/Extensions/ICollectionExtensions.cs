using System.Collections.Generic;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with ICollection objects.
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Adds a <paramref name="value"/> to a <paramref name="collection"/> if it is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The value's type.</typeparam>
        /// <param name="collection">The target collection.</param>
        /// <param name="value">The value to be added.</param>
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value)
            where T : class
        {
            if (collection != null && value != null)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// Adds a <paramref name="value"/> to a <paramref name="collection"/> if it is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The value's type.</typeparam>
        /// <param name="collection">The target collection.</param>
        /// <param name="value">The value to be added.</param>
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