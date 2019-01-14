using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Day17WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static char[,] Map;
        public static int MinYValue = Int32.MaxValue;
        public static int MaxYValue = 0;
        public static int MinXValue = Int32.MaxValue;
        public static int MaxXValue = 0;

        public static int XOffset
        {
            get
            {
                return MinXValue - 2;
            }
        }

        public WriteableBitmap Bitmap { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var lines = Utilities.ReadFile(args[1]);

            ParseInput(lines);

            Bitmap = new WriteableBitmap(MaxXValue + 3 - XOffset, MaxYValue + 1, 96, 96, PixelFormats.Bgr32, null);

            InitializeClay(lines);

            PrintMap(0);

            var display = new Image
            {
                Source = Bitmap,
                Width = Bitmap.PixelWidth,
                Height = Bitmap.PixelHeight
            };
            Root.Children.Add(display);

            bool changed = true;
            int leftBound = 0;
            int rightBound = 0;
            bool leftBounded = true;
            bool rightBounded = true;
            int sideCount = 0;

            Draw(500 + 3 - XOffset, 0, Colors.Blue);
            Map[0, 500 + 3 - XOffset] = '|';

            int steps = 0;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            timer.Start();
            timer.Tick += delegate
            {
                timer.Stop();
                if (changed)
                {
                    steps++;
                    changed = false;
                    for (int count = 0; count < MaxYValue; count++)
                    {
                        for (int innerCount = 1; innerCount <= MaxXValue + 2 - MinXValue; innerCount++)
                        {
                            if (Map[count, innerCount] == '|')
                            {
                                if (Map[count + 1, innerCount] == '.')
                                {
                                    Map[count + 1, innerCount] = '|';
                                    Draw(innerCount, count + 1, Colors.Blue);
                                    changed = true;
                                }
                                else if (Map[count + 1, innerCount] == '#' || Map[count + 1, innerCount] == '~')
                                {
                                    leftBounded = rightBounded = false;
                                    sideCount = 1;
                                    while ((Map[count, innerCount - sideCount] != '#')
                                            && (Map[count + 1, innerCount - sideCount] == '#'
                                        || Map[count + 1, innerCount - sideCount] == '~'))
                                    {
                                        if (Map[count, innerCount - sideCount] == '.')
                                        {
                                            Map[count, innerCount - sideCount] = '|';
                                            Draw(innerCount - sideCount, count, Colors.Blue);
                                            changed = true;
                                        }
                                        sideCount++;
                                    }
                                    if (Map[count, innerCount - sideCount] == '#')
                                    {
                                        leftBounded = true;
                                        leftBound = -sideCount;
                                    }
                                    else
                                    {
                                        if (Map[count, innerCount - sideCount] == '.')
                                        {
                                            Map[count, innerCount - sideCount] = '|';
                                            Draw(innerCount - sideCount, count, Colors.Blue);
                                            changed = true;
                                        }
                                    }
                                    sideCount = 1;
                                    while ((Map[count, innerCount + sideCount] != '#')
                                            && (Map[count + 1, innerCount + sideCount] == '#'
                                        || Map[count + 1, innerCount + sideCount] == '~'))
                                    {
                                        if (Map[count, innerCount + sideCount] == '.')
                                        {
                                            Map[count, innerCount + sideCount] = '|';
                                            Draw(innerCount + sideCount, count, Colors.Blue);
                                            changed = true;
                                        }
                                        sideCount++;
                                    }
                                    if (Map[count, innerCount + sideCount] == '#')
                                    {
                                        rightBounded = true;
                                        rightBound = sideCount;
                                    }
                                    else
                                    {
                                        if (Map[count, innerCount + sideCount] == '.')
                                        {
                                            Map[count, innerCount + sideCount] = '|';
                                            Draw(innerCount + sideCount, count, Colors.Blue);
                                            changed = true;
                                        }
                                    }
                                    if (leftBounded && rightBounded)
                                    {
                                        for (sideCount = leftBound + 1; sideCount < rightBound; sideCount++)
                                        {
                                            if (Map[count, innerCount + sideCount] != '~')
                                            {
                                                Map[count, innerCount + sideCount] = '~';
                                                Draw(innerCount + sideCount, count, Colors.LightBlue);
                                                changed = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    timer.Start();
                }
                else
                {
                    CreateThumbnail("Day17WPF.png", Bitmap.Clone());
                    timer.Stop();
                }
            };

        }

        public void InitializeClay(IList<string> input)
        {
            Map = new char[MaxYValue + 1, MaxXValue + 3 - XOffset];

            for (int j = 0; j < Map.GetLength(0); ++j)
            {
                for (int i = 0; i < Map.GetLength(1); ++i)
                {
                    Map[j, i] = '.';
                }
            }

            // Fill in Map with Clay
            foreach (var line in input)
            {
                var xySplit = line.Split(',');
                if (line.StartsWith("x="))
                {
                    var xCoord = Int32.Parse(xySplit[0].Split('=')[1]) - XOffset;
                    var yCoords = xySplit[1].Split('=')[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = Int32.Parse(yCoords[0]); j <= Int32.Parse(yCoords[1]); j++)
                    {
                        Draw(xCoord, j, Colors.Brown);
                        Map[j, xCoord] = '#';
                    }
                }
                else
                {
                    var yCoord = Int32.Parse(xySplit[0].Split('=')[1]);
                    var xCoords = xySplit[1].Split('=')[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = Int32.Parse(xCoords[0]); i <= Int32.Parse(xCoords[1]); i++)
                    {
                        Draw(i - MinXValue + 2, yCoord, Colors.Brown);
                        Map[yCoord, i - MinXValue + 2] = '#';
                    }
                }
            }
        }

        public void ParseInput(IList<string> input)
        {
            // Figure out size of the grid
            var yCoordSet = new List<int>();
            var xCoordSet = new List<int>();

            foreach (var line in input)
            {
                Regex pattern = new Regex(@"([xy])=(\d+), ([xy])=(\d+)\.\.(\d+)");
                Match m = pattern.Match(line);
                if (m.Groups[1].ToString() == "x")
                {
                    int newMaxYValue = Int32.Parse(m.Groups[5].ToString());
                    MaxYValue = Math.Max(MaxYValue, newMaxYValue);
                    int newMaxXValue = Int32.Parse(m.Groups[2].ToString());
                    MaxXValue = Math.Max(MaxXValue, newMaxXValue);
                    int newMinXValue = Int32.Parse(m.Groups[2].ToString());
                    MinXValue = Math.Min(MinXValue, newMinXValue);
                    int newMinYValue = Int32.Parse(m.Groups[4].ToString());
                    MinYValue = Math.Min(MinYValue, newMinYValue);
                }
                else
                {
                    int newMaxXValue = Int32.Parse(m.Groups[5].ToString());
                    MaxXValue = Math.Max(MaxXValue, newMaxXValue);
                    int newMaxYValue = Int32.Parse(m.Groups[2].ToString());
                    MaxYValue = Math.Max(MaxYValue, newMaxYValue);
                    int newMinYValue = Int32.Parse(m.Groups[2].ToString());
                    MinYValue = Math.Min(MinYValue, newMinYValue);
                    int newMinXValue = Int32.Parse(m.Groups[4].ToString());
                    MinXValue = Math.Min(MinXValue, newMinXValue);
                }
            }
        }
        public void Draw(int x, int y, Color color)
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

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = color_data;
                }
                Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                Bitmap.Unlock();
            }
        }        
        public void FillWithWater()
        {
            bool changed = true;
            int leftBound = 0;
            int rightBound = 0;
            bool leftBounded = true;
            bool rightBounded = true;
            int sideCount = 0;

            Draw(500 - MinXValue + 5, 0, Colors.Blue);
            Map[0, 500 - MinXValue + 3 - XOffset] = '|';

            int steps = 0;
            
            while (changed)
            {
                steps++;
                changed = false;
                for (int count = 0; count < MaxYValue; count++)
                {
                    for (int innerCount = 1; innerCount <= MaxXValue + 2 - MinXValue; innerCount++)
                    {
                        if (Map[count, innerCount] == '|')
                        {
                            if (Map[count + 1, innerCount] == '.')
                            {
                                Map[count + 1, innerCount] = '|';
                                Draw(innerCount, count + 1, Colors.Blue);
                                changed = true;
                            }
                            else if (Map[count + 1, innerCount] == '#' || Map[count + 1, innerCount] == '~')
                            {
                                leftBounded = rightBounded = false;
                                sideCount = 1;
                                while ((Map[count, innerCount - sideCount] != '#')
                                        && (Map[count + 1, innerCount - sideCount] == '#'
                                    || Map[count + 1, innerCount - sideCount] == '~'))
                                {
                                    if (Map[count, innerCount - sideCount] == '.')
                                    {
                                        Map[count, innerCount - sideCount] = '|';
                                        Draw(innerCount - sideCount, count, Colors.Blue);
                                        changed = true;
                                    }
                                    sideCount++;
                                }
                                if (Map[count, innerCount - sideCount] == '#')
                                {
                                    leftBounded = true;
                                    leftBound = -sideCount;
                                }
                                else
                                {
                                    if (Map[count, innerCount - sideCount] == '.')
                                    {
                                        Map[count, innerCount - sideCount] = '|';
                                        Draw(innerCount - sideCount, count, Colors.Blue);
                                        changed = true;
                                    }
                                }
                                sideCount = 1;
                                while ((Map[count, innerCount + sideCount] != '#')
                                        && (Map[count + 1, innerCount + sideCount] == '#'
                                    || Map[count + 1, innerCount + sideCount] == '~'))
                                {
                                    if (Map[count, innerCount + sideCount] == '.')
                                    {
                                        Map[count, innerCount + sideCount] = '|';
                                        Draw(innerCount + sideCount, count, Colors.Blue);
                                        changed = true;
                                    }
                                    sideCount++;
                                }
                                if (Map[count, innerCount + sideCount] == '#')
                                {
                                    rightBounded = true;
                                    rightBound = sideCount;
                                }
                                else
                                {
                                    if (Map[count, innerCount + sideCount] == '.')
                                    {
                                        Map[count, innerCount + sideCount] = '|';
                                        Draw(innerCount + sideCount, count, Colors.Blue);
                                        changed = true;
                                    }
                                }
                                if (leftBounded && rightBounded)
                                {
                                    for (sideCount = leftBound + 1; sideCount < rightBound; sideCount++)
                                    {
                                        if (Map[count, innerCount + sideCount] != '~')
                                        {
                                            Map[count, innerCount + sideCount] = '~';
                                            Draw(innerCount + sideCount, count, Colors.LightBlue);
                                            changed = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void PrintMap(int id)
        {
            using (var sw = new StreamWriter(string.Format("Day17Map{0}.txt", id)))
            {
                for (int j = 0; j < Map.GetLength(0); ++j)
                {
                    for (int i = 0; i < Map.GetLength(1); ++i)
                    {
                        sw.Write(Map[j, i]);
                    }
                    sw.WriteLine();
                }
                sw.WriteLine();
            }
        }

        void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                }
            }
        }
    }
}
