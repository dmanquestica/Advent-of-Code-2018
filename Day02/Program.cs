using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day02
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var tally1 = 0;

            foreach (var s in list)
            {
                foreach (var c in s)
                {
                    //Console.WriteLine(string.Format("Testing char: {0}",c));
                    if (s.Count(ch => ch == c) == 2)
                    {
                        tally1 += 1;
                        //Console.WriteLine("Char found twice");
                        break;
                    }
                    //Console.WriteLine("Not found");
                }
            }

            var tally2 = 0;

            foreach (var s in list)
            {
                foreach (var c in s)
                {
                    //Console.WriteLine(string.Format("Testing char: {0}", c));
                    if (s.Count(ch => ch == c) == 3)
                    {
                        tally2 += 1;
                        //Console.WriteLine("Char found thrice");
                        break;
                    }
                    //Console.WriteLine("Not found");
                }
            }

            Console.WriteLine("Part 1");
            Console.WriteLine(tally1 * tally2);

            Console.WriteLine("Part 2");

            int i, j;

            var listPairs = new List<KeyValuePair<string, string>>();

            while (!listPairs.Any())
            {
                for (i = 0; i < list.Count(); ++i)
                {
                    for (j = 0; j < list.Count(); ++j)
                    {
                        if (DamerauLevenstein(list[i], list[j]) == 1)
                            listPairs.Add(new KeyValuePair<string, string>(list[i], list[j]));
                    }

                }
            }
            //Console.WriteLine(listPairs[0]);

            var result = listPairs[0].Value;

            // Index of the change
            int index = listPairs[0].Key.Zip(listPairs[0].Value, (c1, c2) => c1 == c2).TakeWhile(b => b).Count();

            // Remove to get common string
            Console.WriteLine(result.Remove(index, 1));

            Console.ReadKey();
        }

        /// <summary>
        /// Not mine, finds the edit distance between two strings
        /// </summary>
        /// <param name="original">First String</param>
        /// <param name="modified">Second String</param>
        /// <returns></returns>
        public static int DamerauLevenstein(string original, string modified)
        {
            if (original == modified)
                return 0;

            int len_orig = original.Length;
            int len_diff = modified.Length;
            if (len_orig == 0 || len_diff == 0)
                return len_orig == 0 ? len_diff : len_orig;

            var matrix = new int[len_orig + 1, len_diff + 1];

            for (int i = 1; i <= len_orig; i++)
            {
                matrix[i, 0] = i;
                for (int j = 1; j <= len_diff; j++)
                {
                    int cost = modified[j - 1] == original[i - 1] ? 0 : 1;
                    if (i == 1)
                        matrix[0, j] = j;

                    var vals = new int[] {
                    matrix[i - 1, j] + 1,
                    matrix[i, j - 1] + 1,
                    matrix[i - 1, j - 1] + cost
                };
                    matrix[i, j] = vals.Min();
                    if (i > 1 && j > 1 && original[i - 1] == modified[j - 2] && original[i - 2] == modified[j - 1])
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
                }
            }
            return matrix[len_orig, len_diff];
        }
    }
}
