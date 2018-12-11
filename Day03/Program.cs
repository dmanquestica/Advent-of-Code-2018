using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Day3
{
    public class Program
    {
        public static int SIZE = 1500;

        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var clothList = new List<Cloth>();

            foreach (var s in list)
            {
                clothList.Add(new Cloth(s));
            }

            //Console.WriteLine("Parsed claims");
            
            Console.WriteLine("Part 1");

            int[,] fabric = new int[SIZE, SIZE];
            for (int i = 0; i < SIZE; ++i)
            {
                for (int j = 0; j < SIZE; ++j)
                {
                    fabric[i, j] = 0;
                }
            }

            foreach (Cloth c in clothList)
                AddClaim(ref fabric, c);

            //Console.WriteLine("Added claims");

            var overlap = 0;

            for (int i = 0; i < SIZE; ++i)
            {
                for (int j = 0; j < SIZE; ++j)
                {
                    if (fabric[i, j] > 1)
                        ++overlap;
                }
            }

            Console.WriteLine(overlap);

            Console.WriteLine("Part 2");

            //using (StreamWriter sw = new StreamWriter("Day3Result.txt"))
            //{
            //    for (int i = 0; i < SIZE; ++i)
            //    {
            //        for (int j = 0; j < SIZE; ++j)
            //        {
            //            sw.Write(fabric[i, j].ToString());
            //        }
            //        sw.WriteLine();
            //    }
            //}

            Cloth result = null;

            List<Cloth>.Enumerator clothEnumerable = clothList.GetEnumerator();
            do
            {
                clothEnumerable.MoveNext();
                result = CheckClaim(ref fabric, clothEnumerable.Current);
            } while (result == null);

            Console.WriteLine("#ID: " + result.ID);

            Console.ReadKey();
        }

        public static Cloth CheckClaim(ref int[,] fabric, Cloth c)
        {
            var area = c.Width * c.Height;

            var checkArea = 0;

            for (int i = 0; i < SIZE; ++i)
            {
                for (int j = 0; j < SIZE; ++j)
                {
                    if ((c.Left - 1 < i && i < c.Left + c.Width) && (c.Top - 1 < j && j < c.Top + c.Height))
                        checkArea += fabric[i,j];
                }
            }

            if (area == checkArea)
                return c;
            else
                return null;
        }

        public static void AddClaim(ref int[,] fabric, Cloth c)
        {
            for (int i = 0; i < SIZE; ++i)
            {
                for (int j = 0; j < SIZE; ++j)
                {
                    if ((c.Left - 1 < i && i < c.Left + c.Width) && (c.Top - 1 < j && j < c.Top + c.Height))
                        fabric[i, j] += 1;
                }
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
