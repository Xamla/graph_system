using System;

namespace Xamla.Utilities
{
    public static class DateTimeExtensions
    {
        public static long CompareTo(this DateTime left, DateTime right, long toleranceTicks)
        {
            var d = (left.Ticks / toleranceTicks) - (right.Ticks / toleranceTicks);
            return d == 0 ? 0 : (d < 0 ? -1 : 1);
        }

        public static DateTime Truncate(this DateTime time, long precisionTicks)
        {
            return new DateTime((time.Ticks / precisionTicks) * precisionTicks, time.Kind);
        }

        public static DateTime TruncateToMillisecond(this DateTime time)
        {
            return time.Truncate(TimeSpan.TicksPerMillisecond);
        }

        public static DateTime TruncateToSecond(this DateTime time)
        {
            return time.Truncate(TimeSpan.TicksPerSecond);
        }

        public static DateTime TruncateToMinute(this DateTime time)
        {
            return time.Truncate(TimeSpan.TicksPerMinute);
        }

        public static DateTime TruncateToHour(this DateTime time)
        {
            return time.Truncate(TimeSpan.TicksPerHour);
        }

        public static bool IsSameDay(this DateTime current, DateTime other)
        {
            return current.Year == other.Year 
                && current.Month == other.Month 
                && current.Day == other.Day;
        }
    }
}
