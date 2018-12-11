using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day8
{
    public class Program
    {
        public static int index = 0;

        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var split = list[0].Split(' ').Select(Int32.Parse).ToList();

            var tree = GetNextNode(split);

            Console.WriteLine("Part 1");

            Console.WriteLine(tree.GetMetaSum());

            Console.WriteLine("Part 2");

            Console.WriteLine(tree.GetValue());

            Console.ReadKey();
        }

        public static Node GetNextNode(List<int> numbers)
        {
            var childCount = numbers[index++];
            var metaCount = numbers[index++];

            var result = new Node();

            Enumerable.Range(0, childCount).ToList().ForEach(c => result.Children.Add(GetNextNode(numbers)));
            Enumerable.Range(0, metaCount).ToList().ForEach(c => result.Metadata.Add(numbers[index++]));

            return result;
        }
    }

    public class Node
    {
        public List<int> Metadata = new List<int>();

        public List<Node> Children = new List<Node>();

        public int GetMetaSum()
        {
            return Children.Sum(c => c.GetMetaSum()) + Metadata.Sum();
        }

        public int GetValue()
        {
            if (!Children.Any())
                return Metadata.Sum();
            return Metadata.Where(m => 0 < m && m <= Children.Count).Sum(n => Children[n - 1].GetValue());
        }
    }
}
