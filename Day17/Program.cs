using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day17
{
    public class Program
    {
        public static char[,] Map;
        public static int MinYValue = Int32.MaxValue;
        public static int MaxYValue = 0;
        public static int MinXValue = Int32.MaxValue;
        public static int MaxXValue = 0;

        public static void Main(string[] args)
        {
            var lines = Utilities.ReadFile(args[0]);

            // Figure out size of the grid
            var yCoordSet = new List<int>();
            var xCoordSet = new List<int>();

            foreach (var line in lines)
            {
                Regex pattern = new Regex(@"([xy])=(\d+), ([xy])=(\d+)\.\.(\d+)");
                Match m = pattern.Match(line);
                if (m.Groups[1].ToString() == "x")
                {
                    int newMaxYValue = Int32.Parse(m.Groups[5].ToString());
                    MaxYValue = Math.Max(MaxYValue, newMaxYValue);
                    int newMaxXValue = Int32.Parse(m.Groups[2].ToString());
                    MaxXValue = Math.Max(MaxXValue, newMaxXValue);
                    int newMinXValue = Int32.Parse(m.Groups[2].ToString());
                    MinXValue = Math.Min(MinXValue, newMinXValue);
                    int newMinYValue = Int32.Parse(m.Groups[4].ToString());
                    MinYValue = Math.Min(MinYValue, newMinYValue);
                }
                else
                {
                    int newMaxXValue = Int32.Parse(m.Groups[5].ToString());
                    MaxXValue = Math.Max(MaxXValue, newMaxXValue);
                    int newMaxYValue = Int32.Parse(m.Groups[2].ToString());
                    MaxYValue = Math.Max(MaxYValue, newMaxYValue);
                    int newMinYValue = Int32.Parse(m.Groups[2].ToString());
                    MinYValue = Math.Min(MinYValue, newMinYValue);
                    int newMinXValue = Int32.Parse(m.Groups[4].ToString());
                    MinXValue = Math.Min(MinXValue, newMinXValue);
                }
            }

            Map = new char[MaxYValue + 1, MaxXValue + 3];

            for (int j = 0; j < Map.GetLength(0); ++j)
            {
                for (int i = 0; i < Map.GetLength(1); ++i)
                {
                    Map[j, i] = '.';
                }
            }

            // Fill in Map with Clay
            foreach (var line in lines)
            {
                var xySplit = line.Split(',');
                if (line.StartsWith("x="))
                {
                    var xCoord = Int32.Parse(xySplit[0].Split('=')[1]);
                    var yCoords = xySplit[1].Split('=')[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = Int32.Parse(yCoords[0]); j <= Int32.Parse(yCoords[1]); j++)
                        Map[j, xCoord] = '#';
                }
                else
                {
                    var yCoord = Int32.Parse(xySplit[0].Split('=')[1]);
                    var xCoords = xySplit[1].Split('=')[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = Int32.Parse(xCoords[0]); i <= Int32.Parse(xCoords[1]); i++)
                        Map[yCoord, i] = '#';
                }
            }

            bool changed = true;
            int leftBound = 0;
            int rightBound = 0;
            bool leftBounded = true;
            bool rightBounded = true;
            int sideCount = 0;

            Map[0, 500] = '|';

            int steps = 0;

            PrintMap(steps);

            while (changed)
            {
                steps++;
                changed = false;
                for (int count = 0; count < MaxYValue; count++)
                {
                    PrintMap(steps);
                    for (int innerCount = (MinXValue - 2); innerCount <= MaxXValue + 2; innerCount++)
                    {
                        if (Map[count, innerCount] == '|')
                        {
                            if (Map[count + 1, innerCount] == '.')
                            {
                                Map[count + 1, innerCount] = '|';
                                changed = true;
                            }
                            else if (Map[count + 1, innerCount] == '#' || Map[count + 1, innerCount] == '~')
                            {
                                leftBounded = rightBounded = false;
                                sideCount = 1;
                                while ((Map[count, innerCount - sideCount] != '#')
                                        && (Map[count + 1, innerCount - sideCount] == '#'
                                    || Map[count + 1, innerCount - sideCount] == '~'))
                                {
                                    if (Map[count, innerCount - sideCount] == '.')
                                    {
                                        Map[count, innerCount - sideCount] = '|';
                                        changed = true;
                                    }
                                    sideCount++;
                                }
                                if (Map[count, innerCount - sideCount] == '#')
                                {
                                    leftBounded = true;
                                    leftBound = -sideCount;
                                }
                                else
                                {
                                    if (Map[count, innerCount - sideCount] == '.')
                                    {
                                        Map[count, innerCount - sideCount] = '|';
                                        changed = true;
                                    }
                                }
                                sideCount = 1;
                                while ((Map[count, innerCount + sideCount] != '#') 
                                        && (Map[count + 1, innerCount + sideCount] == '#'
                                    || Map[count + 1, innerCount + sideCount] == '~'))
                                {
                                    if (Map[count, innerCount + sideCount] == '.')
                                    {
                                        Map[count, innerCount + sideCount] = '|';
                                        changed = true;
                                    }
                                    sideCount++;
                                }
                                if (Map[count, innerCount + sideCount] == '#')
                                {
                                    rightBounded = true;
                                    rightBound = sideCount;
                                }
                                else
                                {
                                    if (Map[count, innerCount + sideCount] == '.')
                                    {
                                        Map[count, innerCount + sideCount] = '|';
                                        changed = true;
                                    }
                                }
                                if (leftBounded && rightBounded)
                                {
                                    for (sideCount = leftBound + 1; sideCount < rightBound; sideCount++)
                                    {
                                        if (Map[count, innerCount + sideCount] != '~')
                                        {
                                            Map[count, innerCount + sideCount] = '~';
                                            changed = true;
                                        }
                                    }
                                }
                            }
                        }
                    }       
                }
            }

            PrintMap(steps);

            var total = 0;
            Map[0, 500] = '+';

            for (int j = MinYValue; j < Map.GetLength(0); ++j)
            {
                for (int i = MinXValue - 1; i < Map.GetLength(1); ++i)
                {
                    if (Map[j, i] == '|' || Map[j, i] == '~')
                        total++;
                }
            }

            Console.WriteLine("Part 1");

            Console.WriteLine("Total: {0}", total);

            Console.WriteLine("Part 2");

            total = 0;

            for (int j = MinYValue; j < Map.GetLength(0); ++j)
            {
                for (int i = MinXValue - 1; i < Map.GetLength(1); ++i)
                {
                    if (Map[j, i] == '~')
                        total++;
                }
            }

            Console.WriteLine("Total: {0}", total);

            Console.ReadKey();
        }

        public static void PrintMap(int id)
        {
            using (var sw = new StreamWriter(string.Format("Day17Map{0}.txt", id)))
            {
                for (int j = MinYValue; j < Map.GetLength(0); ++j)
                {
                    for (int i = MinXValue - 1; i < Map.GetLength(1); ++i)
                    {
                        sw.Write(Map[j, i]);
                    }
                    sw.WriteLine();
                }
                sw.WriteLine();
            }
        }
    }
}
