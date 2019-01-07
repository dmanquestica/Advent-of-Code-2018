using Day15WPF;
using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Day18WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static char[,] CombatArea;
        public static List<Unit> Units;
        
        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var lines = Utilities.ReadFile(args[1]);

            ParseInitialState(lines, (bool.Parse(args[2]) ? 16 : 3));

            int turn = 0;

            Refresh(txtDisplay, CombatArea, Units, turn);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Start();
            timer.Tick += delegate
            {
                timer.Stop();

                (int elf, int goblin) = Fight(false);
                if (elf != 0 && goblin != 0)
                {
                    turn++;
                    timer.Start();
                }
                Refresh(txtDisplay, CombatArea, Units, turn);
            };
        }

        public void Refresh(RichTextBox display, char[,] combatArea, List<Unit> units, int turn)
        {
            display.Document.Blocks.Clear();
            var paragraph = new Paragraph
            {
                LineHeight = 6,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight
            };

            var text = CombatAreaToString(combatArea, units, turn);
            for (int i = 0; i < text.Length; ++i)
            {
                switch (text[i])
                {
                    case '#':
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Gray });
                        break;
                    case '.':
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Brown });
                        break;
                    case 'E':
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Green });
                        break;
                    case 'G':
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Red });
                        break;
                    default:
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Black });
                        break;
                }
            }
            display.Document.Blocks.Add(paragraph);
        }
        
        public static (int elf, int goblin) Fight(bool failOnElfDeath)
        {
            Units = Units.OrderBy(u => u.Position.Y).ThenBy(u => u.Position.X).ToList();
            for (int i = 0; i < Units.Count; ++i)
            {
                var targets = Units.Where(t => t.UnitType != Units[i].UnitType).ToList();
                if (targets.Count == 0)
                    return (Units.Where(u => u.UnitType == 'E').Sum(u => u.HP), Units.Where(u => u.UnitType == 'G').Sum(u => u.HP));

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
                    return (Units.Where(u => u.UnitType == 'E').Sum(u => u.HP), Units.Where(u => u.UnitType == 'G').Sum(u => u.HP));

                int index = Units.IndexOf(bestAdjacent);
                Units.RemoveAt(index);
                if (index < i)
                    i--;
            }
            return (Units.Where(u => u.UnitType == 'E').Sum(u => u.HP), Units.Where(u => u.UnitType == 'G').Sum(u => u.HP));
        }

        private static string CombatAreaToString(char[,] combatArea, List<Unit> units, int turn)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);

            var dupArea = (char[,])combatArea.Clone();

            foreach (var c in units)
            {
                dupArea[c.Position.Y, c.Position.X] = c.UnitType;
            }

            for (int i = 0; i < dupArea.GetLength(0); ++i)
            {
                for (int j = 0; j < dupArea.GetLength(1); ++j)
                {
                    sb.Append(dupArea[i, j]);
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine);
            sb.Append($"Turn: {turn} {Environment.NewLine}");
            sb.Append($"Elf HP remaining: {units.Where(u => u.UnitType == 'E').Sum(u => u.HP)}{Environment.NewLine}");
            sb.Append($"Goblin HP remaining: {units.Where(u => u.UnitType == 'G').Sum(u => u.HP)}{Environment.NewLine}");

            return sb.ToString();
        }

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

        public static void ParseInitialState(IList<string> input, int elfAttackPower = 3)
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
}
