using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public static class TimeProvider
    {
        static TimeProvider()
        {
            Reset();
        }

        public static Func<DateTime> Now { get; set; }

        public static void Reset()
        {
            Now = () => DateTime.Now;
        }


        private static long lastTime;
        private static object _timeLock = new object();
        public static DateTime GetCurrentUniversalTime()
        {
            lock (_timeLock)
            {
                // prevent concurrent access to ensure uniqueness 
                DateTime result = DateTime.Now.ToUniversalTime();
                if (result.Ticks <= lastTime)
                {
                    result = new DateTime(lastTime + 1, DateTimeKind.Utc);

                }
                lastTime = result.Ticks;
                return result;
            }
        }
    }

    public static class IdentityGenerator
    {
        public static string GenerateUniqueTickIdentifer()
        {
            long tick = TimeProvider.GetCurrentUniversalTime().Ticks / TimeSpan.TicksPerMillisecond;

            string value = tick.ToString() + RandomString(4) + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();

            return value;
        }

        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
