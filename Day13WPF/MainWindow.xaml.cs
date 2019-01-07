using Day13WPF;
using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Day18WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static char[,] Tracks;

        public static List<Cart> Carts;


        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var lines = Utilities.ReadFile(args[1]);

            int ticks = 0;
            List<Position> allCrashedPositions;

            Carts = null;
            Tracks = InitializeTracks(lines);

            Refresh(txtDisplay, ticks);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            timer.Start();
            timer.Tick += delegate
            {
                timer.Stop();

                if (Carts.Count() > 1 && Carts.Count() % 2 == 1)
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
                                Refresh(txtDisplay, ticks);
                                RemoveCrashedCarts();
                            }
                        }
                    }
                    Refresh(txtDisplay, ticks);
                    // Reorder the carts since they move in sequence from Top to Bottom, Left to Right
                    Carts = Carts.OrderBy(c => c.Position.Y).ThenBy(c => c.Position.X).ToList();
                    ticks++;
                    timer.Start();
                }
            };
        }

        private List<Position> GetAllCrashes()
        {
            var list = new List<Position>();

            var positions = Carts.GroupBy(c => new { c.Position.X, c.Position.Y }).Where(x => x.Count() > 1).Select(x => new { CrashedPosition = x.Key });

            foreach (var p in positions)
                list.Add(new Position() { X = p.CrashedPosition.X, Y = p.CrashedPosition.Y });

            return list;
        }
        private void MarkCartsAsCrashed(List<Position> crashedPositions)
        {
            foreach (var c in Carts.ToList())
                foreach (var p in crashedPositions)
                    if (c.Position.X == p.X && c.Position.Y == p.Y)
                        c.Crashed = true;
        }

        private void RemoveCrashedCarts()
        {
            foreach (var c in Carts.ToList())
                if (c.Crashed)
                    Carts.Remove(c);
        }

        public void AddCart(Direction dir, int x, int y, int id)
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

        public char[,] InitializeTracks(IList<string> input)
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

        public void Refresh(RichTextBox display, int ticks = 0)
        {
            display.Document.Blocks.Clear();
            var paragraph = new Paragraph
            {
                LineHeight = 3,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight
            };
            var text = TracksToString(ticks);

            for (int i = 0; i < text.Length; ++i)
            {
                switch (text[i])
                {
                    case '^':
                    case 'v':
                    case '<':
                    case '>':
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Green, FontWeight = FontWeights.Bold });
                        break;
                    case 'X':
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Red, FontWeight = FontWeights.Bold });
                        break;
                    case ' ':
                        paragraph.Inlines.Add(new Run(".") { Foreground = Brushes.White });
                        break;
                    default:
                        paragraph.Inlines.Add(new Run(text[i].ToString()) { Foreground = Brushes.Black });
                        break;
                }
            }
            display.Document.Blocks.Add(paragraph);
        }

        public string TracksToString(int ticks)
        { 
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);

            var dupTracks = new char[Tracks.GetLength(0), Tracks.GetLength(1)];
            for (int i = 0; i < Tracks.GetLength(0); ++i)
            {
                for (int j = 0; j < Tracks.GetLength(1); ++j)
                {
                    dupTracks[i, j] = Tracks[i, j];
                }
            }

            foreach (var c in Carts.Where(c => c.Crashed == false))
            {
                var cartChar = c.Crashed ? 'X' : c.Direction == Direction.North ? '^' : c.Direction == Direction.South ? 'v' : c.Direction == Direction.East ? '>' : '<';
                dupTracks[c.Position.Y, c.Position.X] = cartChar;
            }

            for (int i = 0; i < dupTracks.GetLength(0); ++i)
            {
                for (int j = 0; j < dupTracks.GetLength(1); ++j)
                {
                    sb.Append(dupTracks[i, j]);
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine);
            sb.Append($"Ticks: {ticks}");

            return sb.ToString();
        }

        public Position CheckForCrashes()
        {
            var positions = Carts.GroupBy(c => new { c.Position.X, c.Position.Y }).Where(x => x.Count() > 1).Select(x => new { Key = x.Key });
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
}
