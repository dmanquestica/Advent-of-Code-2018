using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    public class Program
    {
        public static int INT_OFFSET = 48;
        public static string PUZZLE_INPUT = "633601";

        public static void Main(string[] args)
        {
            Int64 elf1 = 0;
            Int64 elf2 = 1;

            long puzzleInputAsLong = Int64.Parse(PUZZLE_INPUT);


            Int64[] scores = new Int64[puzzleInputAsLong + 10];

            var initialScores = "37";

            long i;

            for (i = 0; i < initialScores.Length; ++i)
                scores[i] = (long)Char.GetNumericValue(initialScores[(int)i]);

            // Make Recipes

            for (; i < scores.Length; ++i)
            {
                var result = scores[elf1] + scores[elf2];
                if (result >= 10)
                {
                    scores[i++] = 1;
                    if (i < scores.Length)
                        scores[i] = result % 10;
                }
                else
                    scores[i] = result;

                elf1 = (elf1 + 1 + scores[elf1]) % (i + 1);
                elf2 = (elf2 + 1 + scores[elf2]) % (i + 1);

            }

            Console.WriteLine("Part 1");

            for (i = puzzleInputAsLong; i < puzzleInputAsLong + 10; ++i)
                Console.Write(scores[i]);
            Console.WriteLine();

            Console.WriteLine("Part 2");

            // Reset everything
            elf1 = 0;
            elf2 = 1;

            long index = 0;
            int positionToCheck = 0;
            bool found = false;

            scores = new Int64[50000000];

            for (i = 0; i < initialScores.Length; ++i)
                scores[i] = (int)Char.GetNumericValue(initialScores[(int)i]);

            while (!found)
            {
                var result = scores[elf1] + scores[elf2];
                if (result >= 10)
                {
                    scores[i++] = 1;
                    if (i < scores.Length)
                        scores[i] = result % 10;
                }
                else
                    scores[i] = result;

                elf1 = (elf1 + 1 + scores[elf1]) % (i + 1);
                elf2 = (elf2 + 1 + scores[elf2]) % (i + 1);

                // Start checking when the first character of the puzzle input appears 
                // plus the length of the puzzle input
                while (index + positionToCheck < i)
                {

                    if ((long)Char.GetNumericValue(PUZZLE_INPUT[positionToCheck]) == scores[index + positionToCheck])
                    {
                        if (positionToCheck == PUZZLE_INPUT.Length - 1)
                        {
                            found = true;
                            break;
                        }
                        positionToCheck++;
                    }
                    else
                    {
                        positionToCheck = 0;
                        index++;
                    }
                }
                i++;
            }

            Console.WriteLine(index);
            Console.ReadKey();
        }
    }
}
