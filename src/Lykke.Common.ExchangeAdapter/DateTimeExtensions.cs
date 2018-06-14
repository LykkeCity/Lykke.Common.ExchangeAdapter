using System;

namespace Lykke.Common.ExchangeAdapter
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime BaseDateTime =
            DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

        public static long Epoch(this DateTime dt)
        {
            return (long) (dt - BaseDateTime).TotalSeconds;
        }

        public static long EpochMilliseconds(this DateTime dt)
        {
            return (long) (dt - BaseDateTime).TotalMilliseconds;
        }

        public static DateTime FromEpoch(this long timestamp)
        {
            return BaseDateTime.AddSeconds(timestamp);
        }
    }
}