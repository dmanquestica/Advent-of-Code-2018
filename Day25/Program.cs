using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day25
{
    public class Program
    {
        public static HashSet<Constellation> Constellations { get; set; }

        public static void Main(string[] args)
        {
            var lines = Utilities.ReadFile(args[0]);

            Constellations = new HashSet<Constellation>();

            var points = ParsePoints(lines);

            Console.WriteLine("Part 1");

            CreateConstellations(points);

            Console.WriteLine(Constellations.Count);

            Console.ReadKey();
        }

        public static void CreateConstellations(IList<Point> points)
        {
            foreach (var p in points)
            {
                var newConstellation = new Constellation();
                newConstellation.AddPoint(p);
                Constellations.Add(newConstellation);
            }

            var continueUnion = true;
            while (continueUnion)
            {
                continueUnion = false;
                foreach (var c in Constellations.ToList())
                {
                    var remainingConstellation = new HashSet<Constellation>(Constellations);
                    foreach (var d in remainingConstellation)
                    {
                        if (c.ShouldUnion(d))
                        {
                            c.Union(d);
                            continueUnion = true;
                        }
                    }
                }
                Constellations = Constellations.Where(c => c.Points.Count > 0).ToHashSet();
            }
        }

        public static List<Point> ParsePoints(IList<string> input)
        {
            var result = new List<Point>();

            foreach (var l in input)
            {
                var pointsSplit = l.Split(',');
                result.Add(new Point()
                {
                    Coords = new int[] {
                        int.Parse(pointsSplit[0]),
                        int.Parse(pointsSplit[1]),
                        int.Parse(pointsSplit[2]),
                        int.Parse(pointsSplit[3])
                    }
                });
            }

            return result;
        }
    }

    public class Constellation
    {
        public HashSet<Point> Points { get; set; }

        public Constellation()
        {
            Points = new HashSet<Point>();
        }

        public bool AddPoint(Point newPoint)
        {
            if (!Points.Any())
                return Points.Add(newPoint);
            else if (Points.Any(p => p.Distance(newPoint) <= 3))
                return Points.Add(newPoint);
            else
                return false;
        }

        public bool ShouldUnion(Constellation otherConstellation)
        {
            var result = false;
            if (this == otherConstellation)
                return result;

            foreach (var p in Points) {
                if (otherConstellation.Points.Any(q => q.Distance(p) <= 3))
                    result = true;
            }
            return result;
        }


        public Constellation Union(Constellation otherConstellation)
        {
            foreach (var p in otherConstellation.Points)
                Points.Add(p);

            otherConstellation.Points.Clear();
            return this;
        }

        public override bool Equals(object obj)
        {
            return !(obj is Constellation item) ? false : Points.SetEquals(item.Points);
        }

        public override int GetHashCode()
        {
            return Points.GetHashCode();
        }
    }

    public class Point : IComparable
    {
        public int[] Coords { get; set; }

        public Point()
        {
        }

        public int Distance(Point otherPoint)
        {
            return Coords.Zip(otherPoint.Coords, (x, y) => (Math.Abs(x - y))).Sum();
        }

        public int CompareTo(object obj)
        {
            return Distance((Point)obj);
        }
    }

}
