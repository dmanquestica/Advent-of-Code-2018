using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day9
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var split = list[0].Split(' ');

            var players = Int32.Parse(split[0]);
            var lastMarble = Int32.Parse(split[6]);

            long[] scores = new long[players];
            LinkedList<int> placedMarbles = new LinkedList<int>();
            LinkedListNode<int> current = placedMarbles.AddFirst(0);

            Console.WriteLine("Part 1");

            Console.WriteLine(PlayGame(players, lastMarble, ref scores, ref placedMarbles, ref current));

            Console.WriteLine("Part 2");

            scores = new long[players];
            placedMarbles = new LinkedList<int>();
            current = placedMarbles.AddFirst(0);

            Console.WriteLine(PlayGame(players, lastMarble * 100, ref scores, ref placedMarbles, ref current));

            Console.ReadKey();
        }

        public static void Next(ref LinkedListNode<int> current, ref LinkedList<int> placed)
        {
            if (current.Next != null)
                current = current.Next;
            else
                current = placed.First;
        }

        public static void Previous(ref LinkedListNode<int> current, ref LinkedList<int> placed)
        {
            if (current.Previous != null)
                current = current.Previous;
            else
                current = placed.Last;
        }

        public static long PlayGame(int players, int lastMarble, ref long[] scores, ref LinkedList<int> placedMarbles, ref LinkedListNode<int> current)
        {
            for (int m = 0; m < lastMarble; ++m)
            {
                if ((m + 1) % 23 == 0)
                {
                    for (int i = 0; i < 7; ++i)
                        Previous(ref current, ref placedMarbles);

                    var j = m % players;
                    scores[j] += m + 1 + current.Value;

                    var tmp = current;
                    Next(ref current, ref placedMarbles);
                    placedMarbles.Remove(tmp);
                }
                else
                {
                    Next(ref current, ref placedMarbles);
                    current = placedMarbles.AddAfter(current, m + 1);
                }
            }

            return scores.Max();
        }
    }


}
