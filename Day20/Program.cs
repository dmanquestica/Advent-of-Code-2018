using SharedUtilities;
using System;
using System.IO;

namespace Day20
{
    public class Program
    {
        public static int maxx = 0, maxy = 0, minx = 2000, miny = 2000;
        public static int[,] dist = new int[2000, 2000];
        public static bool[,] visited = new bool[2000, 2000];
        public static char[,] map = new char[2000, 2000];
        public static String input;  
        public static int maxDistance = 0;

        public static void Main(string[] args)
        {
            input = Utilities.ReadFileAsString(args[0]).Replace("^", string.Empty).Replace("$", string.Empty);

            int x = 1000, y = 1000;
            map[x, y] = 'X';
            BuildPath(x, y, 0, 0);
            OutfileMap();
            Console.WriteLine("Part 1: " + maxDistance);

            var sum = 0;
            for (int j = 0; j < dist.GetLength(0); ++j)
                for (int i = 0; i < dist.GetLength(1); ++i)
                    if (dist[j, i] >= 1000)
                        sum++;

            Console.WriteLine("Part 2: " + sum);
            Console.ReadKey();
        }

        private static void BuildPath(int x, int y, int i, int d)
        {
            while (i < input.Length)
            {
                if (input[i] == 'E')
                {
                    x++;
                    map[x, y] = '|';
                    x++;
                    map[x, y] = '.';

                    maxx = Math.Max(maxx, x);
                }
                else if (input[i] == 'W')
                {
                    x--;
                    map[x, y] = '|';
                    x--;
                    map[x, y] = '.';

                    minx = Math.Min(minx, x);
                }
                else if (input[i] == 'N')
                {
                    y--;
                    map[x, y] = '-';
                    y--;
                    map[x, y] = '.';

                    miny = Math.Min(miny, y);
                }
                else if (input[i] == 'S')
                {
                    y++;
                    map[x, y] = '-';
                    y++;
                    map[x, y] = '.';

                    maxy = Math.Max(maxy, y);
                }
                else if (input[i] == '(')
                {
                    int parentLevel = 0;
                    bool newCondition = true;
                    while (i < input.Length)
                    {
                        i++;
                        if (input[i] == '(')
                            parentLevel++;
                        else if (input[i] == ')')
                        {
                            parentLevel--;
                            if (parentLevel < 0)
                            {
                                BuildPath(x, y, i + 1, d);
                                return;
                            }
                        }
                        else if (input[i] == '|')
                        {
                            if (parentLevel == 0)
                                newCondition = true;
                        }
                        else if (parentLevel == 0)
                        {
                            if (newCondition)
                            {
                                BuildPath(x, y, i, d);
                                newCondition = false;
                            }
                        }
                    }
                }
                else
                    return;
                i++;
                d++;
                if (d >= 1000 && !visited[x, y])
                {
                    visited[x, y] = true;
                }
                if (dist[x, y] == 0 || dist[x, y] > d)
                {
                    dist[x, y] = d;

                    maxDistance = Math.Max(maxDistance, d);
                }
            }
        }

        private static void OutfileMap()
        {
            using (var sw = new StreamWriter("Day20Map.txt"))
            {
                for (int j = minx; j < maxx; j++)
                {
                    sw.Write("#");
                }
                sw.WriteLine("##");
                for (int i = miny; i < maxy; i++)
                {
                    sw.Write("#");
                    for (int j = minx; j < maxx; j++)
                    {
                        if (map[j, i] == 0)
                        {
                            sw.Write("#");
                        }
                        else
                        {
                            sw.Write(map[j, i]);
                        }
                    }
                    sw.Write("#");
                    sw.WriteLine();
                }
                for (int j = minx; j < maxx; j++)
                {
                    sw.Write("#");
                }
                sw.WriteLine("##");
            }
        }

        private static void PrintMap()
        {
            for (int j = minx; j < maxx; j++)
            {
                Console.Write("#");
            }
            Console.WriteLine("##");
            for (int i = miny; i < maxy; i++)
            {
                Console.Write("#");
                for (int j = minx; j < maxx; j++)
                {
                    if (map[j, i] == 0)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(map[j, i]);
                    }
                }
                Console.Write("#");
                Console.WriteLine();
            }
            for (int j = minx; j < maxx; j++)
            {
                Console.Write("#");
            }
            Console.WriteLine("##");
        }

    }
}