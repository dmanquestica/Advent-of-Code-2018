using SharedUtilities;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Day5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            string polymer = list[0].Trim();

            Console.WriteLine("Part 1");

            var input = polymer;

            int inputLength = input.Length;
            input = React(input);
            Console.WriteLine(input.Length);


            Console.WriteLine("Part 2");

            var minLength = polymer.Length;

            for (int u = 97; u <= 122; ++u)
            {
                var input2 = React(Regex.Replace(polymer, ((char)u).ToString(), string.Empty, RegexOptions.IgnoreCase));
                minLength = Math.Min(minLength, input2.Length);
            }
            Console.WriteLine(minLength);

            Console.ReadKey();
        }

        public static string React(string input)
        {
            var sb = new StringBuilder();

            sb.Append(input[0]);
            int i = 1;
            do
            {
                if (sb.Length >= 1 && Math.Abs((int)input[i] - (int)sb[sb.Length - 1]) == 32)
                    sb.Length--;
                else
                    sb.Append(input[i]);
                ++i;
            } while (i < input.Length);

            return sb.ToString();
        }
    }
}
