using System;
using System.Collections.Generic;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with IList objects.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="IList{T}"/>
        /// </summary>
        /// <typeparam name="T">The type of the items in the collections.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="newList">The collection whose elements should be added to the end of the source list.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> newList)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (newList == null)
            {
                throw new ArgumentNullException(nameof(newList));
            }

            if (source is List<T> concreteList)
            {
                concreteList.AddRange(newList);
                return;
            }

            foreach (var element in newList)
            {
                source.Add(element);
            }
        }

        /// <summary>
        /// Shuffles the items in a list randomly.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="list">The list that is going to be shuffled.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var random = new Random();
            for (var i = 0; i < list.Count - 1; i++)
            {
                int r = random.Next(i, list.Count);
                T temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
        }
    }
}