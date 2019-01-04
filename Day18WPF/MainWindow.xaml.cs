using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static ForestMap Map { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var lines = Utilities.ReadFile(args[1]);

            Map = new ForestMap(lines);

            int iterations = 0;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1)};
            timer.Start();
            timer.Tick += delegate
            {
                timer.Stop();
                if (iterations < int.Parse(args[2]))
                {
                    Map.Advance();
                    txtDisplay.Text = Map.ToString();
                    iterations++;
                    timer.Start();
                }
            };
        }

    }


}
