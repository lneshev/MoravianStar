using System;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with DateTime objects.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// This method acts like: "date.DayOfWeek", but it returns the Sunday as 7 instead of 0.
        /// </summary>
        /// <param name="date">The DateTime object.</param>
        /// <returns>Returns the Sunday as 7 instead of 0.</returns>
        public static int DayOfWeekNormalized(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Sunday ? (int)date.DayOfWeek : 7;
        }
    }
}