using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day6
{
    public class Program
    {
        public const int MARGIN = 0;

        public const int XSIZE = 360;
        public const int YSIZE = 360;
        public const int MAXDISTANCE = 10000;

        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var points = ParsePoints(list);

            int[,] gridDist = new int[XSIZE, YSIZE];
            int[,] gridOwner = new int[XSIZE, YSIZE];
            int[,] totalDistance = new int[XSIZE, YSIZE]; // For Part 2 Only

            for (int i = 0; i < gridDist.GetLength(0); ++i)
            {
                for (int j = 0; j < gridDist.GetLength(1); ++j)
                {
                    gridDist[i, j] = Int32.MaxValue;
                }
            }

            for (int i = 0; i < gridOwner.GetLength(0); ++i)
            {
                for (int j = 0; j < gridOwner.GetLength(1); ++j)
                {
                    gridOwner[i, j] = -1;
                }
            }

            var minX = points.Min(p => p.Item1);
            var minY = points.Min(p => p.Item2);

            var maxX = points.Max(p => p.Item1);
            var maxY = points.Max(p => p.Item2);

            int[] owned = new int[points.Count];
            bool[] infinite = new bool[points.Count];

            var rangeY = Tuple.Create(minY - MARGIN, maxY + MARGIN);
            var rangeX = Tuple.Create(minX - MARGIN, maxX + MARGIN);

            // Find distance from point
            for (int p = 0; p < points.Count; ++p)
            {
                for (int i = 0; i < gridDist.GetLength(0); ++i)
                {
                    for (int j = 0; j < gridDist.GetLength(1); ++j)
                    {
                        var dist = Math.Abs(points[p].Item1 - i) + Math.Abs(points[p].Item2 - j);
                        totalDistance[i, j] += dist; // For Part 2 Only
                        if (dist < gridDist[i, j])
                        {
                            gridDist[i, j] = dist;
                            gridOwner[i, j] = p;
                        }
                        else if (dist == gridDist[i, j])
                            gridOwner[i, j] = -1;
                    }
                }
            }

            //using (StreamWriter sw = new StreamWriter("Day6Map.txt"))
            //{
            //    for (int i = 0; i < XSIZE; ++i)
            //    {
            //        for (int j = 0; j < YSIZE; ++j)
            //        {
            //            sw.Write(gridOwner[i, j] == -1 ? "    " : "[" + gridOwner[i, j].ToString("D2") + "]");
            //        }
            //        sw.WriteLine();
            //    }
            //}

            Tuple<int, int> bestPoint = Tuple.Create(0, 0);

            // Find the infinite points (edge)
            var row = 0;
            for (int j = 0; j < gridOwner.GetLength(1); ++j)
            {
                if (gridOwner[row, j] != -1)
                    infinite[gridOwner[row, j]] = true;
            }
            var col = 0;
            for (int i = 0; i < gridOwner.GetLength(0); ++i)
            {
                if (gridOwner[i, col] != -1)
                    infinite[gridOwner[i, col]] = true;
            }

            row = rangeX.Item2;
            for (int j = 0; j < gridOwner.GetLength(1); ++j)
            {
                if (gridOwner[row, j] != -1)
                    infinite[gridOwner[row, j]] = true;
            }
            col = rangeY.Item2;
            for (int i = 0; i < gridOwner.GetLength(0); ++i)
            {
                if (gridOwner[i, col] != -1)
                    infinite[gridOwner[i, col]] = true;
            }

            // Count number of point per owner
            for (int p = 0; p < points.Count; ++p)
            {
                for (int i = 0; i < gridOwner.GetLength(0); ++i)
                {
                    for (int j = 0; j < gridOwner.GetLength(1); ++j)
                    {
                        if (gridOwner[i, j] == p)
                        {
                            owned[p] += 1;
                        }
                    }
                }
            }

            Console.WriteLine("Part 1");

            var zip = owned.Zip(infinite, (a, b) => (b ? 0 : a));

            Console.WriteLine("Largest Region: " + zip.Max());

            //for (int i = 0; i < owned.Length; ++i)
            //{
            //    Console.WriteLine("Point {3}: {{{0}, {1}}} owning {2} points {4}", points[i].Item1, points[i].Item2, owned[i], i, infinite[i] ? "Infinite" : string.Empty);
            //}

            Console.WriteLine("Part 2");

            int withinSafeRegion = 0;

            for (int i = 0; i < totalDistance.GetLength(0); ++i)
            {
                for (int j = 0; j < totalDistance.GetLength(1); ++j)
                {
                    if (totalDistance[i, j] < MAXDISTANCE)
                        withinSafeRegion++;
                }
            }

            //using (StreamWriter sw = new StreamWriter("Day6Map2.txt"))
            //{
            //    for (int i = 0; i < XSIZE; ++i)
            //    {
            //        for (int j = 0; j < YSIZE; ++j)
            //        {
            //            sw.Write(totalDistance[i, j] < MAXDISTANCE ? "*" : " ");
            //        }
            //        sw.WriteLine();
            //    }
            //}


            Console.WriteLine("Size of safe distance: " + withinSafeRegion);
            Console.ReadKey();
        }

        public static List<Tuple<int, int>> ParsePoints(IList<string> list)
        {
            var result = new List<Tuple<int, int>>();

            foreach (var s in list)
            {
                var split = s.Split(new string[] { ", " }, StringSplitOptions.None);
                result.Add(Tuple.Create(Int32.Parse(split[0]), Int32.Parse(split[1])));
            }
            return result;
        }
    }
}
