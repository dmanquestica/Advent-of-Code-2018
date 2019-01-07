using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day15
{
    public class Program
    {
        public static char[,] CombatArea;
        public static List<Unit> Units;

        public static void Main(string[] args)
        {
            var input = Utilities.ReadFile(args[0]);

            Console.WriteLine("Part 1");

            Console.WriteLine("Outcome {0}", Fight(input, 3, false));

            Console.WriteLine("Part 2");

            for (int attackPower = 4; ; attackPower++)
            {
                int? outcome = Fight(input, attackPower, true);
                if (outcome.HasValue)
                {
                    Console.WriteLine("Outcome {0}", outcome);
                    break;
                }
            }

            Console.ReadKey();

        }

        public static int? Fight(IList<string> input, int elfAttackPower, bool failOnElfDeath)
        {
            ParseInitialState(input, elfAttackPower);

            for (int turn = 0; ; turn++)
            {
                Units = Units.OrderBy(u => u.Position.Y).ThenBy(u => u.Position.X).ToList();
                for (int i = 0; i < Units.Count; ++i)
                {
                    var targets = Units.Where(t => t.UnitType != Units[i].UnitType).ToList();
                    if (targets.Count == 0)
                        return turn * Units.Sum(ru => ru.HP);

                    if (!targets.Any(t => Units[i].InRange(t)))
                        TryMove(Units[i], targets);

                    var bestAdjacent = targets
                        .Where(t => t.InRange(Units[i]))
                        .OrderBy(t => t.HP)
                        .ThenBy(t => t.Position.Y)
                        .ThenBy(t => t.Position.X)
                        .FirstOrDefault();

                    if (bestAdjacent == null)
                        continue;

                    bestAdjacent.HP -= Units[i].AttackPower;
                    if (bestAdjacent.HP > 0)
                        continue; // Skip below and restart loop

                    // Remove dead units
                    if (failOnElfDeath && bestAdjacent.UnitType == 'E') // If an Elf dies, then return null
                        return null;

                    int index = Units.IndexOf(bestAdjacent);
                    Units.RemoveAt(index);
                    if (index < i)
                        i--;
                }

                // Print out current state
                //PrintCombatArea(turn);
            }
        }

        private static void PrintCombatArea(int turn)
        {
            var dupArea = new char[CombatArea.GetLength(0), CombatArea.GetLength(1)];
            for (int i = 0; i < CombatArea.GetLength(0); ++i)
            {
                for (int j = 0; j < CombatArea.GetLength(1); ++j)
                {
                    dupArea[i, j] = CombatArea[i, j];
                }
            }

            foreach (var c in Units)
            {
                dupArea[c.Position.Y, c.Position.X] = c.UnitType;
            }

            using (var sw = new StreamWriter(string.Format("Day15Turn{0}.txt", turn)))
            {
                for (int i = 0; i < dupArea.GetLength(0); ++i)
                {
                    for (int j = 0; j < dupArea.GetLength(1); ++j)
                    {
                        sw.Write(dupArea[i, j]);
                    }
                    sw.WriteLine();
                }
                sw.WriteLine();
                sw.WriteLine(turn);
                sw.WriteLine("Elf HP remaining: {0}", Units.Where(u => u.UnitType == 'E').Sum(u => u.HP));
                sw.WriteLine("Goblin HP remaining: {0}", Units.Where(u => u.UnitType == 'G').Sum(u => u.HP));
            }
        }

        // Up, Left, Right Down
        private static readonly (int dx, int dy)[] inRangeDirections = { (0, -1), (-1, 0), (1, 0), (0, 1) };
        private static void TryMove(Unit u, List<Unit> targets)
        {
            // Get list of inRange positions for each target
            var inRange = new HashSet<(int x, int y)>();
            foreach (var target in targets)
            {
                foreach ((int dx, int dy) in inRangeDirections)
                {
                    (int nx, int ny) = (target.Position.X + dx, target.Position.Y + dy);
                    if (IsOpen(nx, ny))
                        inRange.Add((nx, ny));
                }
            }

            // Perform Breadth-First Search against Open Positions
            var queue = new Queue<(int x, int y)>();
            var prevPositions = new Dictionary<(int x, int y), (int px, int py)>();

            queue.Enqueue((u.Position.X, u.Position.Y));
            prevPositions.Add((u.Position.X, u.Position.Y), (-1, -1));
            while (queue.Count > 0)
            {
                (int x, int y) = queue.Dequeue();
                foreach ((int dx, int dy) in inRangeDirections)
                {
                    (int x, int y) neighbour = (x + dx, y + dy);
                    if (prevPositions.ContainsKey(neighbour) || !IsOpen(neighbour.x, neighbour.y))
                        continue;
                    queue.Enqueue(neighbour);
                    prevPositions.Add(neighbour, (x, y));
                }
            }

            // Local method to get path from "in-range" spaces to current unit (then reversed to get path)
            List<(int x, int y)> getPath(int destX, int destY)
            {
                if (!prevPositions.ContainsKey((destX, destY)))
                    return null;
                List<(int x, int y)> path = new List<(int x, int y)>();
                (int x, int y) = (destX, destY);
                while (x != u.Position.X || y != u.Position.Y)
                {
                    path.Add((x, y));
                    (x, y) = prevPositions[(x, y)];
                }
                path.Reverse();
                return path;
            }

            // Sorting the best path by read-order
            List<(int tx, int ty, List<(int x, int y)> path)> paths =
                inRange
                .Select(t => (t.x, t.y, path: getPath(t.x, t.y)))
                .Where(t => t.path != null)
                .OrderBy(t => t.path.Count)
                .ThenBy(t => t.y)
                .ThenBy(t => t.x)
                .ToList();

            List<(int x, int y)> bestPath = paths.FirstOrDefault().path;
            if (bestPath != null)
                (u.Position.X, u.Position.Y) = bestPath[0];
        }

        public static bool IsOpen(int x, int y)
        {
            return CombatArea[y, x] == '.' && Units.All(u => u.Position.X != x || u.Position.Y != y);
        }

        public static void ParseInitialState(IList<string> input, int elfAttackPower)
        {
            var width = input[0].Length;
            var height = input.Count();

            CombatArea = new char[height, width];
            Units = new List<Unit>();

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    switch (input[i][j])
                    {
                        case 'E':
                            CreateElf(j, i, elfAttackPower);
                            CombatArea[i, j] = '.';
                            break;
                        case 'G':
                            CreateGoblin(j, i);
                            CombatArea[i, j] = '.';
                            break;
                        default:
                            CombatArea[i, j] = input[i][j];
                            break;
                    }
                }
            }
        }

        public static void CreateElf(int x, int y, int attackPower)
        {
            Units.Add(new Elf() { AttackPower = attackPower, Position = new Position() { X = x, Y = y } });
        }

        public static void CreateGoblin(int x, int y)
        {
            Units.Add(new Goblin() { Position = new Position() { X = x, Y = y } });
        }
    }

    public class Elf : Unit
    {
        public Elf()
        {
            UnitType = 'E';
        }
    }

    public class Goblin : Unit
    {
        public Goblin()
        {
            UnitType = 'G';
        }
    }

    public class Unit : IComparable
    {
        public char UnitType { get; set; }
        public int HP { get; set; }
        public int AttackPower { get; set; }
        public Position Position { get; set; }

        public bool Alive() { return (HP > 0); }

        public Unit(int attackPower = 3)
        {
            HP = 200;
            AttackPower = attackPower;
        }

        public bool InRange(Unit otherUnit)
        {
            return Position.IsAdjacent(otherUnit.Position);
        }

        public int CompareTo(object obj)
        {
            return this.Position.CompareTo(((Unit)obj).Position);
        }
    }

    public class Position : IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position()
        {
        }

        public bool IsAdjacent(Position otherPosition)
        {
            return (Math.Abs(Y - otherPosition.Y) + Math.Abs(X - otherPosition.X) == 1);
        }

        public int CompareTo(object obj)
        {
            if (this.Y.CompareTo(((Position)obj).Y) != 0)
                return this.Y.CompareTo(((Position)obj).Y);
            else if (this.X.CompareTo(((Position)obj).X) != 0)
                return this.X.CompareTo(((Position)obj).X);
            else
                return 0;
        }
    }
}
