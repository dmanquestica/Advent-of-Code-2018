using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventofCodeDay1Part2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var freqs = new List<int>();
            foreach (var s in list)
            {
                freqs.Add(Int32.Parse(s));
            }

            Console.WriteLine("Part 1");

            Console.WriteLine(freqs.Sum());

            var currentFreq = 0;

            Console.WriteLine("Part 2");

            var existingFreqs = new HashSet<int>();

            int iterations = 0;
            for (int i = 0; i < freqs.Count(); ++i)
            {
                //Console.WriteLine(string.Format("Adding {0} to {1}", freqs[i], currentFreq));
                currentFreq += freqs[i];
                if (existingFreqs.Contains(currentFreq))
                {
                    Console.WriteLine(currentFreq);
                    break;
                }
                else
                {
                    //Console.WriteLine(string.Format("Current Frequency: {0}", currentFreq));
                    existingFreqs.Add(currentFreq);
                }
                if (i == freqs.Count - 1)
                {
                    //Console.WriteLine("Repeating list");
                    i = -1;
                }
            }

            Console.WriteLine("Iterations {0}", iterations);
            Console.ReadKey();
        }
    }
}
