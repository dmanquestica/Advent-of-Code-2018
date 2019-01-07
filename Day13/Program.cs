using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day13
{
    public class Program
    {
        public static char[,] Tracks;

        public static List<Cart> Carts;

        public static void Main(string[] args)
        {
            var input = Utilities.ReadFile(args[0]);

            Tracks = DrawTracks(input);

            //WriteMapToFile();

            Console.WriteLine("Part 1");

            int ticks = 0;
            List<Position> allCrashedPositions;
            do
            {
                foreach (var c in Carts)
                {
                    if (c.Crashed == false)
                        c.Move(ref Tracks);
                    else
                        throw new Exception("Cart crashed!");
                }
                ticks++;
                allCrashedPositions = GetAllCrashes();
                if (allCrashedPositions.Any())
                    MarkCartsAsCrashed(allCrashedPositions);
            } while (!allCrashedPositions.Any() && Carts.Any());

            Console.WriteLine("{0},{1}", allCrashedPositions.First().X, allCrashedPositions.First().Y);

            Console.WriteLine("Part 2");

            // Reset everything
            Carts = null;
            Tracks = DrawTracks(input);
            ticks = 0;

            //WriteMapToFile(ticks);

            while (Carts.Count() > 1 && Carts.Count() % 2 == 1)
            {
                foreach (var c in Carts.ToList())
                {
                    if (c.Crashed == false)
                    {
                        // Move a cart, and check for collision before continuing
                        c.Move(ref Tracks);
                        allCrashedPositions = GetAllCrashes();
                        if (allCrashedPositions.Any())
                        {
                            MarkCartsAsCrashed(allCrashedPositions);
                            //if (ticks <= 2000)
                            //    WriteMapToFile(ticks);
                            RemoveCrashedCarts();
                            Console.WriteLine("Crash! Remaining cart count {0}", Carts.Count());
                        }
                    }
                }
                // Reorder the carts since they move in sequence from Top to Bottom, Left to Right
                Carts = Carts.OrderBy(c => c.Position.Y).ThenBy(c => c.Position.X).ToList();
                ticks++;
            }
            if (Carts.Count() == 1) 
                Console.WriteLine(Carts.ToList()[0].Position);

            Console.ReadKey();
        }

        private static List<Position> GetAllCrashes()
        {
            var list = new List<Position>();

            var positions = Carts.GroupBy(c => new { c.Position.X, c.Position.Y }).Where(x => x.Count() > 1).Select(x => new { CrashedPosition = x.Key });

            foreach (var p in positions)
                list.Add(new Position() { X = p.CrashedPosition.X, Y = p.CrashedPosition.Y });

            return list;
        }
        private static void MarkCartsAsCrashed(List<Position> crashedPositions)
        {
            foreach (var c in Carts.ToList())
                foreach (var p in crashedPositions)
                    if (c.Position.X == p.X && c.Position.Y == p.Y)
                        c.Crashed = true;
        }


        private static void RemoveCrashedCarts()
        {
            foreach (var c in Carts.ToList())
                if (c.Crashed)
                    Carts.Remove(c);
        }

        public static void AddCart(Direction dir, int x, int y, int id)
        {
            if (Carts == null)
                Carts = new List<Cart>();

            Carts.Add(new Cart()
            {
                ID = id,
                Direction = dir,
                Position = new Position() { X = x, Y = y }
            });
        }

        public static char[,] DrawTracks(IList<string> input)
        {
            var width = input.Max(c => c.Length);
            var height = input.Count();

            var map = new char[height, width];
            var id = 0;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    switch (input[i][j])
                    {
                        case '^':
                            AddCart(Direction.North, j, i, id);
                            id++;
                            map[i, j] = '|';
                            break;
                        case 'v':
                            AddCart(Direction.South, j, i, id);
                            id++;
                            map[i, j] = '|';
                            break;
                        case '>':
                            AddCart(Direction.East, j, i, id);
                            id++;
                            map[i, j] = '-';
                            break;
                        case '<':
                            AddCart(Direction.West, j, i, id);
                            id++;
                            map[i, j] = '-';
                            break;
                        default:
                            map[i, j] = input[i][j];
                            break;
                    }
                }
            }

            Carts = Carts.OrderBy(c => c.Position.Y).ThenBy(c => c.Position.X).ToList();
            return map;
        }

        public static void WriteMapToFile(int ticks = 0)
        {
            var dupTracks = new char[Tracks.GetLength(0), Tracks.GetLength(1)];
            for (int i = 0; i < Tracks.GetLength(0); ++i)
            {
                for (int j = 0; j < Tracks.GetLength(1); ++j)
                {
                    dupTracks[i,j] =Tracks[i, j];
                }
            }

            foreach (var c in Carts.Where(c => c.Crashed == false))
            {
                var cartChar = c.Crashed ? 'X' : (c.ID % 10).ToString()[0]; // c.Direction == Direction.North ? '^' : c.Direction == Direction.South ? 'v' : c.Direction == Direction.East ? '>' : '<';
                dupTracks[c.Position.Y, c.Position.X] = cartChar;
            }

            using (var sw = new StreamWriter(string.Format("Day13Map{0}.txt", ticks)))
            {
                for (int i = 0; i < dupTracks.GetLength(0); ++i)
                {
                    for (int j = 0; j < dupTracks.GetLength(1); ++j)
                    {
                        sw.Write(dupTracks[i, j]);
                    }
                    sw.WriteLine();
                }
                sw.WriteLine();
                sw.WriteLine(ticks);
            }            
        }

        public static Position CheckForCrashes()
        {
            var positions = Carts.GroupBy(c => new { c.Position.X, c.Position.Y } ).Where(x => x.Count() > 1).Select(x => new { Key = x.Key });
            if (positions.Any())
            {
                foreach (var c in Carts)
                    if (c.Position.X == positions.ToList()[0].Key.X && c.Position.Y == positions.ToList()[0].Key.Y)
                        c.Crashed = true;
                return new Position() { X = positions.ToList()[0].Key.X, Y = positions.ToList()[0].Key.Y };
            }
            return null;
        }
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", X, Y);
        }
    }

    public class Cart
    {
        public int ID { get; set; }
        public Direction Direction { get; set; }
        public Position Position { get; set; }
        public bool Crashed { get; set; }
        public int TimesAtIntersection { get; set; }

        public Cart()
        {
            Crashed = false;
            TimesAtIntersection = 0;
        }
        
        public void TurnRight()
        {
            if (Direction == Direction.North)
                Direction = Direction.East;
            else if (Direction == Direction.South)
                Direction = Direction.West;
            else if (Direction == Direction.West)
                Direction = Direction.North;
            else
                Direction = Direction.South;
        }

        public void TurnLeft()
        {
            if (Direction == Direction.North)
                Direction = Direction.West;
            else if (Direction == Direction.South)
                Direction = Direction.East;
            else if (Direction == Direction.West)
                Direction = Direction.South;
            else
                Direction = Direction.North;
        }

        public void IntersectionMove(ref char[,] Tracks)
        {
            switch (TimesAtIntersection % 3 )
            {
                case 0:
                    TurnLeft();
                    break;
                case 1:
                    break;
                case 2:
                    TurnRight();
                    break;
            }
            TimesAtIntersection++;
        }

        public void Move(ref char[,] Tracks)
        {
            if (Direction == Direction.North || Direction == Direction.South)
            {
                var positionToCheck = Direction == Direction.North ? Tracks[Position.Y - 1, Position.X] : Tracks[Position.Y + 1, Position.X];
                Position.Y -= Direction == Direction.North ? 1 : -1;
                switch (positionToCheck)
                {
                    case '|':
                        break;
                    case '\\':
                        TurnLeft();
                        break;
                    case '/':
                        TurnRight();
                        break;
                    case '+':
                        IntersectionMove(ref Tracks);
                        break;
                    default:
                        throw new Exception("You're off the track");
                }
            }
            else if (Direction == Direction.East || Direction == Direction.West)
            {
                var positionToCheck = Direction == Direction.East ? Tracks[Position.Y, Position.X + 1] : Tracks[Position.Y, Position.X - 1];
                Position.X += Direction == Direction.East ? 1 : -1;
                switch (positionToCheck)
                {
                    case '-':
                        break;
                    case '\\':
                        TurnRight();
                        break;
                    case '/':
                        TurnLeft();
                        break;
                    case '+':
                        IntersectionMove(ref Tracks);
                        break;
                    default:
                        throw new Exception("You're off the track");
                }
            }
        }
    }

    public enum Direction
    {
        North = 0,
        South = 1,
        West = 2,
        East = 3,
    }
}
