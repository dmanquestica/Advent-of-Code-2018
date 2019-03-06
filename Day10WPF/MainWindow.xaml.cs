using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Day10WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int SIZE = 750;
        public static int OFFSET = 200;
        public WriteableBitmap Bitmap { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var lines = Utilities.ReadFile(args[1]);

            Bitmap = new WriteableBitmap(SIZE, SIZE, 96, 96, PixelFormats.Bgr32, null);

            var points = new List<MessagePoint>();

            foreach (var s in lines)
            {
                var temp = CreateMessagePoint(s);
                if (temp != null)
                    points.Add(temp);
            }

            var display = new Image
            {
                Source = Bitmap,
                Width = Bitmap.PixelWidth,
                Height = Bitmap.PixelHeight
            };
            Root.Children.Add(display);

            int minX = points.Min(p => p.X);
            int maxX = points.Max(p => p.X);

            int minY = points.Min(p => p.Y);
            int maxY = points.Max(p => p.Y);

            int rangeY = maxY - minY;
            int previousRangeY = Int32.MaxValue;

            while (!(minY + OFFSET >= 0 && maxY + OFFSET < SIZE && minX + OFFSET >= 0 && maxX + OFFSET < SIZE))
            {
                foreach (var p in points)
                {
                    p.Forward();
                }
                minX = points.Min(p => p.X);
                maxX = points.Max(p => p.X);

                minY = points.Min(p => p.Y);
                maxY = points.Max(p => p.Y);

                rangeY = maxY - minY;
            }

            int secondsNeeded = 0; // For Part 2
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            timer.Start();
            timer.Tick += delegate
            {
                timer.Stop();

                if (previousRangeY > rangeY)
                {
                    previousRangeY = points.Max(p => p.Y) - points.Min(p => p.Y);
                    foreach (var p in points)
                    {
                        Draw(p.X + OFFSET, p.Y + OFFSET, Colors.Blue);
                        p.Forward();
                    }

                    minY = points.Min(p => p.Y);
                    maxY = points.Max(p => p.Y);

                    rangeY = maxY - minY;
                    if (rangeY > previousRangeY) // Moved too far, so reverse 1 second
                    {
                        foreach (var p in points)
                        {
                            p.Reverse();
                            Draw(p.X + OFFSET, p.Y + OFFSET, Colors.Green);
                        }
                        timer.Stop();
                    }
                    else
                    {
                        timer.Start();
                    }
                    secondsNeeded++;  // For Part 2
                }

                minX = points.Min(p => p.X);
                maxX = points.Max(p => p.X);

                minY = points.Min(p => p.Y);
                maxY = points.Max(p => p.Y);
            };
        }

        public MessagePoint CreateMessagePoint(string s)
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

        private void Draw(int x, int y, Color color)
        {
            try
            {
                Bitmap.Lock();

                unsafe
                {
                    int pBackBuffer = (int)Bitmap.BackBuffer;
                    pBackBuffer += y * Bitmap.BackBufferStride;
                    pBackBuffer += x * 4;

                    int color_data = color.R << 16; // R
                    color_data |= color.G << 8;   // G
                    color_data |= color.B << 0;   // B

                    *((int*)pBackBuffer) = color_data;
                }
                Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                Bitmap.Unlock();
            }
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
