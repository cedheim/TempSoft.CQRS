using System.Threading;

namespace TempSoft.CQRS.Common.Random
{
    public class ThreadSafeRandom
    {
        private static readonly System.Random GlobalRandom = new System.Random();
        private static readonly object GlobalLock = new object();
        private static readonly ThreadLocal<System.Random> ThreadRandom = new ThreadLocal<System.Random>(NewRandom);

        public static System.Random Instance => ThreadRandom.Value;

        private static System.Random NewRandom()
        {
            lock (GlobalLock)
            {
                return new System.Random(GlobalRandom.Next());
            }
        }

        public static int Next()
        {
            return Instance.Next();
        }

        public static int Next(int maxValue)
        {
            return Instance.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Instance.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return Instance.NextDouble();
        }

        public static void NextBytes(byte[] buffer)
        {
            Instance.NextBytes(buffer);
        }
    }
}