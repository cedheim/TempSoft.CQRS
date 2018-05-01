using System;

namespace TempSoft.CQRS.Common.Extensions
{
    public static class LongExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this long unixTime)
        {
            return UnixEpoch.AddSeconds(unixTime);
        }
    }
}