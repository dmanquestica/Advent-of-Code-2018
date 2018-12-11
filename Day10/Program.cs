using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var points = new List<MessagePoint>();

            foreach (var s in list)
            {
                var temp = CreateMessagePoint(s);
                if (temp != null)
                    points.Add(temp);
            }

            int minX = points.Min(p => p.X);
            int maxX = points.Max(p => p.X);

            int minY = points.Min(p => p.Y);
            int maxY = points.Max(p => p.Y);

            int rangeY = maxY - minY;
            int previousRangeY = Int32.MaxValue;

            int secondsNeeded = 0; // For Part 2
            while (previousRangeY > rangeY)
            {
                previousRangeY = points.Max(p => p.Y) - points.Min(p => p.Y);
                foreach (var p in points)
                    p.Forward();

                minY = points.Min(p => p.Y);
                maxY = points.Max(p => p.Y);

                rangeY = maxY - minY;
                if (rangeY > previousRangeY) // Moved too far, so reverse 1 second
                {
                    foreach (var p in points)
                    {
                        p.Reverse();
                    }
                    break;
                }
                secondsNeeded++;  // For Part 2
            }

            Console.WriteLine("Part 1");

            minX = points.Min(p => p.X);
            maxX = points.Max(p => p.X);

            minY = points.Min(p => p.Y);
            maxY = points.Max(p => p.Y);

            for (int j = minY; j <= maxY; ++j)
            {
                for (int i = minX; i <= maxX; ++i)
                {
                    Console.Write((points.FirstOrDefault(p => p.X == i && p.Y == j) != null ? "#" : " "));
                }
                Console.WriteLine();
            }
            Console.WriteLine(); // Purely for formatting

            Console.WriteLine("Part 2");

            Console.WriteLine("Seconds Needed: " + secondsNeeded);

            Console.ReadKey();
        }

        public static MessagePoint CreateMessagePoint(string s)
        {
            int posX, posY, speedX, speedY;
            var pattern = @"^position=<( )*(-?\d+),( )*(-?\d+)> velocity=<( )*(-?\d+),( )*(-?\d+)>$";
            Regex r = new Regex(pattern, RegexOptions.None);
            Match m = r.Match(s);
            while (m.Success)
            {
                posX = Int32.Parse(m.Groups[2].ToString());
                posY = Int32.Parse(m.Groups[4].ToString());
                speedX = Int32.Parse(m.Groups[6].ToString());
                speedY = Int32.Parse(m.Groups[8].ToString());

                return new MessagePoint(posX, posY, speedX, speedY);
            }
            return null;
        }
    }

    public class MessagePoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Velocity Velocity { get; set; }

        public MessagePoint(int x, int y, int speedX, int speedY)
        {
            X = x;
            Y = y;
            Velocity = new Velocity(speedX, speedY);
        }

        public void Forward()
        {
            X = X + Velocity.X;
            Y = Y + Velocity.Y;
        }

        public void Reverse()
        {
            X = X - Velocity.X;
            Y = Y - Velocity.Y;
        }
    }

    public class Velocity
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Velocity(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
