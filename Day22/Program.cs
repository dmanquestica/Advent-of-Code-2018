using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day22
{
    public class Program
    {
        public static char[,] Map;
        public static long[,] GIndex;
        public static long[,] ErosionLevel;
        public static long Depth;
        public static (int X, int Y) Target;

        public static (int x, int y)[] Directions = { (-1, 0), (0, 1), (1, 0), (0, -1) };

        public static long MODULO = 20183;

        public static Stack<(int x, int y, long minutes)> Path;

        public static void Main(string[] args)
        {
            var input = Utilities.ReadFile(args[0]);

            BuildMap(input);

            OutfileMap();

            Console.WriteLine("Part 1");

            var riskLevel = 0;
            for (int y = 0; y <= Target.Y; ++y) {
                for (int x = 0; x <= Target.X; ++x) {
                    switch (Map[y, x])
                    {
                        case '.':
                            riskLevel += 0;
                            break;
                        case '=':
                            riskLevel += 1;
                            break;
                        case '|':
                            riskLevel += 2;
                            break;
                    }
                }
            }

            Console.WriteLine(riskLevel);

            Console.WriteLine("Part 2");

            BreadthFirstSearch();

            Console.ReadKey();
        }
        
        public static void BreadthFirstSearch()
        {

            Queue<(int x, int y, char tool, int switching, int minutes)> vertices = new Queue<(int x, int y, char tool, int switching, int minutes)>();
            HashSet<(int x, int y, char tool)> visited = new HashSet<(int x, int y, char tool)>();

            vertices.Enqueue((0, 0, 'T', 0, 0)); // Equip torch

            visited.Add((0, 0, 'T'));
            Path.Push((0, 0, 0));

            while (vertices.Count > 0)
            {
                (int x, int y, char tool, int switching, int minutes) = vertices.Dequeue();
                if (switching > 0)
                {
                    if (switching != 1 || visited.Add((x, y, tool)))
                        vertices.Enqueue((x, y, tool, switching - 1, minutes + 1));
                    continue;
                }
                if ((x == Target.X && y == Target.Y) && (tool == 'T'))
                {
                    Console.WriteLine(minutes);
                    break;
                }

                foreach ((int dx, int dy) in Directions)
                {
                    (int newX, int newY) = (x + dx, y + dy);
                    if (newX < 0 || newY < 0) 
                        continue;

                    if (GetAllowedTools(Map[newY, newX]).Contains(tool) && visited.Add((newX, newY, tool)))
                    {
                        vertices.Enqueue((newX, newY, tool, 0, minutes + 1));
                    }
                }

                foreach (char otherTool in GetAllowedTools(Map[y, x]))
                {
                    vertices.Enqueue((x, y, otherTool, 6, minutes + 1));
                }
            }
        }

        private static string GetAllowedTools(char region)
        {
            switch (region)
            {
                case '.': return "CT";
                case '=': return "CN";
                case '|': return "TN";
                default: throw new Exception("Unreachable");
            }
        }

        public static void BuildMap(IList<string> input) 
        {
            Depth = Int32.Parse(input[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
            var TargetSplit = input[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            Target = (Int32.Parse(TargetSplit.Split(',')[0]), Int32.Parse(TargetSplit.Split(',')[1]));

            var height = Target.Y + 20;
            var width = Target.X * 56;

            Map = new char[height, width];
            GIndex = new long[height, width];
            ErosionLevel = new long[height, width];

            Path = new Stack<(int x, int y, long minutes)>();
            
            for (long y = 0; y < Map.GetLength(0); ++y)
            {
                for (long x = 0; x < Map.GetLength(1); ++x)
                {
                    long localGIndex = 0;
                    if ((y == 0 && x == 0) || (y == Target.Y && x == Target.X))
                        localGIndex = 0;
                    else if (y == 0 && x != 0)
                        localGIndex = 16807 * x;
                    else if (x == 0 && y != 0)
                        localGIndex = 48271 * y;
                    else
                        localGIndex = ErosionLevel[y - 1, x] * ErosionLevel[y, x - 1];
                    GIndex[y, x] = localGIndex;

                    ErosionLevel[y, x] = (GIndex[y, x] + Depth) % MODULO;

                    switch (ErosionLevel[y, x] % 3)
                    {
                        case 0:
                            Map[y, x] = '.';
                            break;
                        case 1:
                            Map[y, x] = '=';
                            break;
                        case 2:
                            Map[y, x] = '|';
                            break;
                    }
                }
            }
        }

        public static void PrintMap()
        {
            for (int y = 0; y < Map.GetLength(0); ++y)
            {
                for (int x = 0; x < Map.GetLength(1); ++x)
                {
                    Console.Write(Map[y, x]);
                }
                Console.WriteLine();
            }
        }

        private static void OutfileMap()
        {
            using (var sw = new StreamWriter("Day22Map.txt"))
            {
                for (int y = 0; y < Map.GetLength(0); ++y)
                {
                    for (int x = 0; x < Map.GetLength(1); ++x)
                    {
                        if (y == 0 && x == 0)
                            sw.Write('M');
                        else if (y == Target.Y && x == Target.X)
                            sw.Write('T');
                        else
                            sw.Write(Map[y, x]);
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}
