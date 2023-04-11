using System;
using System.Collections.Generic;

namespace MoravianStar.Extensions
{
    public static class IListExtensions
    {
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