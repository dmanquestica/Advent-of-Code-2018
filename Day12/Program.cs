using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var input = Utilities.ReadFile(args[0]);

            var plants = new HashSet<int>();

            var rules = new Dictionary<int, bool>();

            // Initial Pots State
            var initialState = input[0].Split(':')[1].Trim();

            initialState.Select((pot, index) => new { pot, index })
                .Where(p => p.pot == '#')
                .Select(p => p.index)
                .ToList().ForEach(pot => plants.Add(pot));

            // Set up all rules
            for (int i = 2; i < input.Count(); ++i)
            {
                int rule = input[i]
                    .Take(5)
                    .Select((pot, index) => new { pot, index })
                    .Where(c => c.pot == '#')
                    .Sum(c => (int)Math.Pow(2, c.index)); // Convert pattern to binary
                rules.Add(rule, input[i].EndsWith("#") ? true : false);
            }

            Console.WriteLine("Part 1");

            long iterations = 20;
            long totalSum = 0;
            HashSet<int> newPlants;

            for (int iter = 1; iter <= iterations; iter++)
            {
                // Perform the comparison
                newPlants = new HashSet<int>();
                int min = plants.Min() - 3;
                int max = plants.Max() + 3;

                for (int pot = min; pot <= max; pot++)
                {
                    int sum = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (plants.Contains(pot + i - 2))
                            sum += (int)Math.Pow(2, i); // Convert plant and neighbours string to binary
                    }
                    if (rules.Keys.Contains(sum) && rules[sum]) // If the binary exists, and leads to a plant in the next generation
                        newPlants.Add(pot);                     // Add to list of new plants
                }
                plants = newPlants;
                totalSum = plants.Sum();
            }

            Console.WriteLine(totalSum);

            Console.WriteLine("Part 2");

            plants = new HashSet<int>();
            // Reset Initial List of Plants
            initialState.Select((pot, index) => new { pot, index })
                .Where(p => p.pot == '#')
                .Select(p => p.index)
                .ToList().ForEach(pot => plants.Add(pot));

            iterations = 50000000000;
            totalSum = 0;

            for (long iter = 1; iter <= iterations; iter++)
            {
                newPlants = new HashSet<int>();
                int min = plants.Min() - 3;
                int max = plants.Max() + 3;

                for (int pot = min; pot <= max; pot++)
                {
                    int sum = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (plants.Contains(pot + i - 2))
                            sum += (int)Math.Pow(2, i);
                    }
                    if (rules.Keys.Contains(sum) && rules[sum])
                        newPlants.Add(pot);
                }
                // Found pattern stabilized after approx 175 iterations, but plant location shifted by 1 each time
                if (iter > 175 && plants.Select(x => x + 1).Except(newPlants).Count() == 0)
                {
                    plants = newPlants;
                    totalSum = plants.Sum();
                    totalSum += plants.Count() * (iterations - iter); // Since increases are constant  at this point, just calculate remaining iterations of plants
                    break;
                }
                else
                {
                    plants = newPlants;
                }
            }

            Console.WriteLine(totalSum);

            Console.ReadKey();
        }
    }
}
