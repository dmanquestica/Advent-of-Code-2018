using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day11
{
    public class Program
    {
        public static int[,] Grid { get; set; }

        public static int MaxAvailableSize { get; set; }

        public static void Main(string[] args)
        {
            var serialNumber = Int32.Parse(args[0]);

            Console.WriteLine("Part 1");

            MaxAvailableSize = 3;

            Grid = new int[301, 301];

            int maxPower = Int32.MinValue;
            int xCoord = 0;
            int yCoord = 0;
            int size = 0;

            for (int y = 1; y < Grid.GetLength(0); y++)
            {
                for (int x = 1; x < Grid.GetLength(1); ++x)
                {
                    Grid[y, x] = PowerFormula(serialNumber, x, y) + Grid[y - 1, x] + Grid[y, x - 1] - Grid[y - 1, x - 1];
                }
            }

            for (int y = MaxAvailableSize; y < Grid.GetLength(0); y++)
            {
                for (int x = MaxAvailableSize; x < Grid.GetLength(1); x++)
                {
                    int total = Grid[y,x] - Grid[y - MaxAvailableSize, x] - Grid[y,x - MaxAvailableSize] + Grid[y - MaxAvailableSize, x - MaxAvailableSize];
                    if (total > maxPower)
                    {
                        maxPower = total;
                        xCoord = x;
                        yCoord = y;
                        size = MaxAvailableSize;
                    }
                }
            }

            Console.WriteLine("X,Y is {0},{1} with a power of {2}", xCoord - size + 1, yCoord - size + 1, Grid[xCoord, yCoord]);

            Console.WriteLine("Part 2");

            maxPower = Int32.MinValue;
            xCoord = 0;
            yCoord = 0;
            MaxAvailableSize = 300;
            size = 0;

            for (int s = 1; s <= MaxAvailableSize; s++)
            {
                for (int y = s; y <= 300; y++)
                {
                    for (int x = s; x <= 300; x++)
                    {
                        int total = Grid[y, x] - Grid[y - s, x] - Grid[y, x - s] + Grid[y - s, x - s];
                        if (total > maxPower)
                        {
                            maxPower = total;
                            xCoord = x;
                            yCoord = y;
                            size = s;
                        }
                    }
                }
            }
            Console.WriteLine("X,Y,Size is {0},{1},{2} with a power of {3}", xCoord - size + 1, yCoord - size + 1, size, Grid[xCoord, yCoord]);

            Console.ReadKey();
        }

        public static int PowerFormula(int gridSerialNumber, int x, int y)
        {
            var rackID = x + 10;
            var powerLevel = (rackID * y + gridSerialNumber) * rackID / 100 % 10 - 5;

            return powerLevel;
        }
    }
}
