using System;
using System.Collections.Generic;

namespace MoravianStar.Extensions
{
    public static class IListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
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