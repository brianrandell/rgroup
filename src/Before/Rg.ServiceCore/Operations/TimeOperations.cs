using System;

namespace Rg.ServiceCore.Operations
{
    public static class TimeOperations
    {
        public static string GetTimeAgo(DateTime utc)
        {
            if (utc.Kind == DateTimeKind.Unspecified)
            {
                // EF likes to give them to us in this format
                utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            }
            TimeSpan time = DateTime.UtcNow - utc;
            if (time < TimeSpan.Zero)
            {
                time = TimeSpan.Zero;
            }

            if (time.TotalDays >= 1)
            {
                return RoundAndPluralize(time.TotalDays, "day");
            }
            if (time.TotalHours >= 1)
            {
                return RoundAndPluralize(time.TotalHours, "hour");
            }
            if (time.TotalMinutes >= 1)
            {
                return RoundAndPluralize(time.TotalMinutes, "minute");
            }
            if (time.TotalSeconds >= 1)
            {
                return RoundAndPluralize(time.TotalSeconds, "second");
            }

            return "just now";
        }

        public static string RoundAndPluralize(double duration, string unit)
        {
            var value = (int) duration;
            string baseText = value.ToString() + " " + unit;
            return value > 1 ? baseText + "s" : baseText;
        }
    }
}
