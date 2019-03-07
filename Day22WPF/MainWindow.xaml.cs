using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Day22WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int RESIZE = 2;

        // For display
        public WriteableBitmap Bitmap { get; set; }

        // For solving
        public char[,] Map;
        public long[,] GIndex;
        public long[,] ErosionLevel;
        public long Depth;
        public (int X, int Y) Target;

        public (int x, int y)[] Directions = { (-1, 0), (0, 1), (1, 0), (0, -1) };

        public long MODULO = 20183;

        public Stack<(int x, int y, char tool, int prvX, int prvY)> ReversePath;

        public Stack<(int x, int y, char tool)> ForwardPath;

        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var input = Utilities.ReadFile(args[1]);

            BuildMap(input);

            var display = new Image
            {
                Source = Bitmap,
                Width = Bitmap.PixelWidth,
                Height = Bitmap.PixelHeight
            };
            Root.Children.Add(display);

            DrawMap();

            MarkTarget();

            MarkCurrent(0, 0);

            BreadthFirstSearch();

            (int x, int y) dest = (Target.X, Target.Y);

            ForwardPath = new Stack<(int x, int y, char tool)>();

            while (ReversePath.Count() > 0)
            {
                var current = ReversePath.Pop();
                if (current.x == dest.x && current.y == dest.y)
                {
                    if (current.prvX >= 0 && current.prvY >= 0)
                        ForwardPath.Push((current.prvX, current.prvY, current.tool));
                    dest = (current.prvX, current.prvY);
                }
            }

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            timer.Start();
            timer.Tick += delegate
            {
                timer.Stop();
                if (ForwardPath.Any())
                {
                    var (x, y, tool) = ForwardPath.Pop();
                    MarkPath(x, y, tool);
                    timer.Start();
                }

            };
        }

        public void BreadthFirstSearch()
        {
            Queue<(int x, int y, char tool, int switching, int minutes)> vertices = new Queue<(int x, int y, char tool, int switching, int minutes)>();
            HashSet<(int x, int y, char tool)> visited = new HashSet<(int x, int y, char tool)>();

            vertices.Enqueue((0, 0, 'T', 0, 0)); // Equip torch

            visited.Add((0, 0, 'T'));
            ReversePath.Push((0, 0, 'T', -1, -1));

            while (vertices.Count > 0)
            {
                (int x, int y, char tool, int switching, int minutes) = vertices.Dequeue();
                if (switching > 0)
                {
                    if (switching != 1 || visited.Add((x, y, tool)))
                        vertices.Enqueue((x, y, tool, switching - 1, minutes + 1));
                    continue;
                }
                if ((x == Target.X && y == Target.Y) && (tool == 'T'))
                    break;

                foreach ((int dx, int dy) in Directions)
                {
                    (int newX, int newY) = (x + dx, y + dy);
                    if (newX < 0 || newY < 0)
                        continue;

                    if (GetAllowedTools(Map[newY, newX]).Contains(tool) && visited.Add((newX, newY, tool)))
                    {
                        vertices.Enqueue((newX, newY, tool, 0, minutes + 1));
                        ReversePath.Push((newX, newY, tool, x, y));
                    }
                }

                foreach (char otherTool in GetAllowedTools(Map[y, x]))
                    vertices.Enqueue((x, y, otherTool, 6, minutes + 1));
            }
        }

        private string GetAllowedTools(char region)
        {
            switch (region)
            {
                case '.': return "CT";
                case '=': return "CN";
                case '|': return "TN";
                default: throw new Exception("Unreachable");
            }
        }

        public void BuildMap(IList<string> input)
        {
            Depth = Int32.Parse(input[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
            var TargetSplit = input[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            Target = (Int32.Parse(TargetSplit.Split(',')[0]), Int32.Parse(TargetSplit.Split(',')[1]));

            var height = Target.Y + 20;
            var width = Target.X * 56;

            Map = new char[height, width];
            GIndex = new long[height, width];
            ErosionLevel = new long[height, width];
            Bitmap = new WriteableBitmap(width * RESIZE, height * RESIZE, 96, 96, PixelFormats.Bgr32, null);
            
            ReversePath = new Stack<(int x, int y, char tool, int prvX, int prvY)>();

            for (long y = 0; y < Map.GetLength(0); ++y)
            {
                for (long x = 0; x < Map.GetLength(1); ++x)
                {
                    long localGIndex = 0;
                    if ((y == 0 && x == 0) || (y == Target.Y && x == Target.X))
                        localGIndex = 0;
                    else if (y == 0 && x != 0)
                        localGIndex = 16807 * x;
                    else if (x == 0 && y != 0)
                        localGIndex = 48271 * y;
                    else
                        localGIndex = ErosionLevel[y - 1, x] * ErosionLevel[y, x - 1];
                    GIndex[y, x] = localGIndex;

                    ErosionLevel[y, x] = (GIndex[y, x] + Depth) % MODULO;

                    switch (ErosionLevel[y, x] % 3)
                    {
                        case 0:
                            Map[y, x] = '.';
                            break;
                        case 1:
                            Map[y, x] = '=';
                            break;
                        case 2:
                            Map[y, x] = '|';
                            break;
                    }
                }
            }
        }

        public void MapPixel(WriteableBitmap bitmap, int x, int y, Color color)
        {
            for (var i = x * RESIZE; i < x * RESIZE + RESIZE; ++i)
            {
                for (var j = y * RESIZE; j < y * RESIZE + RESIZE; ++j)
                {
                    Utilities.DrawPixel(Bitmap, i, j, color);
                }
            }
        }

        public void MarkCurrent(int x, int y)
        {
            MapPixel(Bitmap, x, y, Colors.Lime);
        }
        
        public void MarkPath(int x, int y, char tool)
        {
            switch (tool)
            {
                case 'T':
                    MapPixel(Bitmap, x, y, Colors.OrangeRed);
                    break;
                case 'C':
                    MapPixel(Bitmap, x, y, Colors.Purple);
                    break;
                default:
                    MapPixel(Bitmap, x, y, Colors.Pink);
                    break;
            }
        }

        public void MarkTarget()
        {
            MapPixel(Bitmap, Target.X, Target.Y, Colors.Red);
        }

        public void DrawMap()
        {
            for (int y = 0; y < Map.GetLength(0); ++y)
            {
                for (int x = 0; x < Map.GetLength(1); ++x)
                {
                    switch (Map[y, x])
                    {
                        case '.':
                            MapPixel(Bitmap, x, y, Colors.Gray);
                            break;
                        case '=':
                            MapPixel(Bitmap, x, y, Colors.DarkBlue);
                            break;
                        case '|':
                            MapPixel(Bitmap, x, y, Colors.Black);
                            break;
                    }
                }
            }
        }
    }
}
