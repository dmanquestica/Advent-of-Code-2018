using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day23
{
    public class Program
    {
        public static List<Nanobot> Bots;

        public static int BestCount;
        public static long CurrentX;
        public static long CurrentY;
        public static long CurrentZ;

        public static void Main(string[] args)
        {
            var lines = Utilities.ReadFile(args[0]);

            Bots = ParseInput(lines);

            var botWithStrongestSignal = Bots.Where(b => b.SignalRadius == Bots.Max(s => s.SignalRadius)).First();

            Console.WriteLine("Part 1");

            var inRange = Bots.Count(bot => Distance((bot.X, bot.Y, bot.Z), 
                (botWithStrongestSignal.X, botWithStrongestSignal.Y, botWithStrongestSignal.Z)) 
                <= botWithStrongestSignal.SignalRadius);

            Console.WriteLine(inRange);

            Console.WriteLine("Part 2");

            // Start at average coordinates

            CurrentX = Bots.Sum(b => b.X) / Bots.Count;
            CurrentY = Bots.Sum(b => b.Y) / Bots.Count;
            CurrentZ = Bots.Sum(b => b.Z) / Bots.Count;

            BestCount = CountInRange((CurrentX, CurrentY, CurrentZ));

            var maxStep = Math.Max(CurrentX, Math.Max(CurrentY, CurrentZ));

            // Jump around to find local maximum
            var step = maxStep;
            var iteration = 0;
            var bestPreviousStep = BestCount;
            while (step > 0) {
                Console.WriteLine($"Attempt ${iteration}: {BestCount} bots @ (X, Y, Z) ({CurrentX}, {CurrentY}, {CurrentZ})");
                StepIterate(step);
                step /= 2;
                iteration++;
            }

            Console.WriteLine(Math.Abs(CurrentX) + Math.Abs(CurrentY) + Math.Abs(CurrentZ));
            Console.ReadKey();
        }

        private static void StepIterate(long stepSize)
        {
            var steps = new (long x, long y, long z)[] {
                (1, 1, 1), (1, 1, 0), (1, 0, 1),
                (0, 1, 1), (1, 0, 0), (0, 1, 0),
                (0, 0, 1) };

            foreach (var (x, y, z) in steps)
            {
                var xDir = (CurrentX > 0 ? -stepSize : stepSize) * x;
                var yDir = (CurrentY > 0 ? -stepSize : stepSize) * y;
                var zDir = (CurrentZ > 0 ? -stepSize : stepSize) * z;

                while (IsGood((CurrentX + xDir, CurrentY + yDir, CurrentZ + zDir)))
                {
                    CurrentX += xDir;
                    CurrentY += yDir;
                    CurrentZ += zDir;
                }
            }
        }

        private static List<Nanobot> ParseInput(IList<string> lines)
        {
            var result = new List<Nanobot>();

            var pattern = @"pos=<([-]?\d+),([-]?\d+),([-]?\d+)>, r=(\d+)";

            foreach (var l in lines)
            {
                var regex = new Regex(pattern);
                var m = regex.Match(l);
                var tempBot = new Nanobot() {
                    X = Int64.Parse(m.Groups[1].Value),
                    Y = Int64.Parse(m.Groups[2].Value),
                    Z = Int64.Parse(m.Groups[3].Value),
                    SignalRadius = Int64.Parse(m.Groups[4].Value)
                };
                result.Add(tempBot);
            }
            return result;
        }

        private static bool IsGood((long x, long y, long z) point)
        {
            var tempInRange = CountInRange(point);
            BestCount = Math.Max(BestCount, tempInRange);
            return tempInRange == BestCount;
        }

        private static int CountInRange((long x, long y, long z) point)
        {
            return Bots.Count(bot => Distance((bot.X, bot.Y, bot.Z), (point.x, point.y, point.z)) <= bot.SignalRadius);
        }

        private static long Distance((long x, long y, long z) a, (long x, long y, long z) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }
    }

    public class Nanobot
    {

        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
        public long SignalRadius { get; set; }

        public Nanobot()
        {
        }
    }    
}
