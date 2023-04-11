using System;

namespace MoravianStar.Extensions
{
    public static class DateTimeExtensions
    {
        public static int DayOfWeekNormalized(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Sunday ? (int)date.DayOfWeek : 7;
        }
    }
}