using System;

namespace TempSoft.CQRS.Common.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime time)
        {
            var utcTime = time.ToUniversalTime();
            return (long) utcTime.Subtract(UnixEpoch).TotalSeconds;
        }
    }
}