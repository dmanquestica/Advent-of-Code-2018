using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day7
{
    public class Program
    {
        public const int OFFSET = 65;
        public static StringBuilder order = new StringBuilder();
        public static int timeTaken = 0;
        public static Dictionary<char, List<char>> allSteps;
        public static SortedSet<char> availableSteps;
        public static Dictionary<char, int> inProgressStep;

        public static void Main(string[] args)
        {

            Console.WriteLine("Part 1");

            DoWork(args[0], 1, 1);

            Console.WriteLine("Part 2");

            DoWork(args[0], 5, 60);

            Console.ReadKey();
        }


        public static void DoWork(string file, int workers, int secondsPerStep)
        {
            allSteps = new Dictionary<char, List<char>>();
            availableSteps = new SortedSet<char>();
            inProgressStep = new Dictionary<char, int>();

            var timeTaken = 0;
            var order = new StringBuilder();
            var availWorkers = new Queue<int>(Enumerable.Range(0, workers));

            // Building the graph
            BuildGraph(Utilities.ReadFile(file));

            // Start working
            do
            {
                timeTaken++;
                while (availableSteps.Count > 0 && availWorkers.Count > 0) // While there are available steps AND available workers
                {
                    char nextItem = availableSteps.First(); // Get the first
                    order.Append(nextItem); // Put the first job in line to do
                    availableSteps.Remove(nextItem); // Remove it from the available jobs
                    inProgressStep.Add(nextItem, nextItem + secondsPerStep - OFFSET); // Track the remaining time needed left in the job
                    availWorkers.Dequeue(); // Use one of the workers
                }

                var ipCopy = new Dictionary<char, int>(inProgressStep);
                foreach (var item in ipCopy) // Used to progress each step in parallel
                {
                    char currItem = item.Key; 
                    if (item.Value == 0) // If the progress remaining for this step is zero
                    {
                        inProgressStep.Remove(currItem); // Remove it from the steps list
                        availWorkers.Enqueue(0); // Make another worker available
                        var stepsCopy = new Dictionary<char, List<char>>(allSteps); 
                        foreach (var kvp in stepsCopy) // Get a copy of the list of steps
                        {
                            var list = kvp.Value; // Get the dependent step
                            if (list.Contains(currItem)) 
                            {
                                list.Remove(currItem); // Remove the precursor step
                                allSteps[kvp.Key] = list; // Update the actual list
                                if (list.Count == 0) // If there are no longer precursor steps left
                                    availableSteps.Add(kvp.Key); // Make this step available
                            }
                        }
                    }
                    else
                        inProgressStep[currItem] = item.Value - 1; // Still progress need to be made, so just decrement the progress value
                }
            } while (availableSteps.Count > 0 || inProgressStep.Count > 0); // While there are available steps to do, or while progress still needs to be made

            Console.WriteLine(order.ToString());
            Console.WriteLine(timeTaken.ToString());
            Console.WriteLine();
        }

        public static void BuildGraph(IList<string> instructions)
        {
            foreach (string instruction in instructions)
            {
                char precursor = char.Parse(instruction.Split(' ')[1]);
                char dependent = char.Parse(instruction.Split(' ')[7]);
                if (allSteps.ContainsKey(dependent)) // if the step has already been created
                {
                    allSteps[dependent].Add(precursor); // add this as a precursor
                    availableSteps.Remove(dependent); // Since it has a precursor step, remove it from available steps
                }
                else
                    allSteps.Add(dependent, new List<char> { precursor }); // it doesn't exist so add it, and add the precursor
                if (!allSteps.ContainsKey(precursor)) // if the precursor doesn't exists as a step
                {
                    allSteps.Add(precursor, new List<char>()); // add it as a step 
                    availableSteps.Add(precursor); // and since it doesn't have a precursor, it should be available to do be worked on first
                }
            }
        }
    }
}
