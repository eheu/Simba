using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simba
{
    static class Util
    {
        public class Random
        {
            // Singleton pattern:
            private static Random uniqueInstance;

            // Holds the instance of System.Random
            private readonly System.Random systemRandom;

            // Singleton pattern: Private constructor.
            private Random()
            {
                systemRandom = new System.Random(unchecked((int)DateTime.Now.Ticks));
            }

            public static Random Instance()
            {
                return uniqueInstance ?? (uniqueInstance = new Random());
            }

            public int Next(int lower, int upper)
            {
                // delegate to systemRandom
                return systemRandom.Next(lower, upper);
            }

        }


    }
}
