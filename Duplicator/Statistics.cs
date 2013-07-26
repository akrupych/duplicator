using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Duplicator
{
    class Statistics
    {
        private static Stopwatch Timer { get; set; }

        private static List<long> Checkouts { get; set; }

        private static Dictionary<string, int> Works { get; set; }

        public static void Reset()
        {
            Timer = new Stopwatch();
            Checkouts = new List<long>();
            Works = new Dictionary<string, int>();
        }

        public static void StartTimer()
        {
            Timer.Start();
        }

        public static void Checkout()
        {
            Checkouts.Add(Timer.ElapsedMilliseconds);
        }

        public static void StopTimer()
        {
            Timer.Stop();
        }

        public static void Increment(string work)
        {
            if (Works.ContainsKey(work)) Works[work]++;
            else Works.Add(work, 1);
        }

        public static int GetWork(string work)
        {
            return Works[work];
        }

        public static long GetTime()
        {
            return Timer.ElapsedMilliseconds;
        }
    }
}
