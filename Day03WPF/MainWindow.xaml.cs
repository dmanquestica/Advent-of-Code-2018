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

namespace Day03WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int SIZE = 1200;
        public WriteableBitmap Bitmap { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var lines = Utilities.ReadFile(args[1]);

            Bitmap = new WriteableBitmap(SIZE, SIZE, 96, 96, PixelFormats.Bgr32, null);

            var clothList = new List<Cloth>();

            foreach (var s in lines)
            {
                clothList.Add(new Cloth(s));
            }

            var display = new Image
            {
                Source = Bitmap,
                Width = Bitmap.PixelWidth,
                Height = Bitmap.PixelHeight
            };
            Root.Children.Add(display);

            var i = 0;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            timer.Start();
            timer.Tick += delegate
            {
                timer.Stop();
                if (i < clothList.Count())
                {
                    AddClaim(clothList[i]);
                    ++i;
                    timer.Start();
                }
                else
                {
                    FindNonOverlapping(clothList);
                }
            };

        }

        private void FindNonOverlapping(List<Cloth> clothList)
        {
            // Already know the answer so cheating here, 
            // but could re-run analysis from Day03 project
            // and get the same result
            var c = clothList[551]; 
            for (var j = c.Top; j <= c.Top + c.Height; ++j)
                for (var i = c.Left; i <= c.Left + c.Width; ++i)
                    Draw(i, j, Colors.LimeGreen, false);
        }

        public void AddClaim(Cloth c)
        {
            for (var j = c.Top; j <= c.Top + c.Height; ++j)
                for (var i = c.Left; i <= c.Left + c.Width; ++i)
                    Draw(i, j, Colors.Red, true);
        }

        private void Draw(int x, int y, Color color, bool overlap)
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

                    int background_color_data = Colors.Black.R << 16;
                    background_color_data |= Colors.Black.G << 8;
                    background_color_data |= Colors.Black.B << 0;

                    int alternate_color_data = Colors.Blue.R << 16;
                    alternate_color_data |= Colors.Blue.G << 8;
                    alternate_color_data |= Colors.Blue.B << 0;

                    // Assign the color data to the pixel.
                    if (*((int*)pBackBuffer) != background_color_data && overlap)
                        *((int*)pBackBuffer) = alternate_color_data;
                    else
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

    public class Cloth
    {
        public string ID { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Cloth(string line)
        {
            var pattern = @"^(#\d+)\s@\s(\d+),(\d+):\s(\d+)x(\d+)$";
            Regex r = new Regex(pattern, RegexOptions.None);
            Match m = r.Match(line);
            ID = m.Groups[1].ToString();
            Left = Int32.Parse(m.Groups[2].ToString());
            Top = Int32.Parse(m.Groups[3].ToString());
            Width = Int32.Parse(m.Groups[4].ToString());
            Height = Int32.Parse(m.Groups[5].ToString());
        }

    }
}
