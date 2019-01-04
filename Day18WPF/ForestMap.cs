using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day18WPF
{
    public class ForestMap : IEquatable<ForestMap>
    {
        public long Minute { get; private set; }

        private static readonly (int dx, int dy)[] AdjacentDirections = { (-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1) };

        public char[,] State { get; set; }
        public char[,] OriginalState { get; set; }

        public ForestMap()
        {
            Minute = 0;
        }

        public ForestMap(IList<string> lines)
        {
            Minute = 0;

            State = new char[lines.Count, lines[0].Length];

            for (int y = 0; y < lines.Count; ++y)
            {
                for (int x = 0; x < lines[0].Length; ++x)
                {
                    State[y, x] = lines[y][x];
                }
            }
            OriginalState = (char[,])State.Clone();
        }

        public void Reset()
        {
            Minute = 0;
            State = (char[,])OriginalState.Clone();
        }

        public int CountAcre(char c)
        {
            var acre = 0;

            for (int y = 0; y < State.GetLength(0); ++y)
            {
                for (int x = 0; x < State.GetLength(1); ++x)
                {
                    if (State[y, x] == c)
                        acre++;
                }
            }

            return acre;
        }

        public int CountResource()
        {
            return CountAcre('|') * CountAcre('#');
        }

        public void Advance()
        {
            var tempMap = new char[State.GetLength(0), State.GetLength(1)];

            for (int y = 0; y < State.GetLength(0); ++y)
            {
                for (int x = 0; x < State.GetLength(1); ++x)
                {
                    var adjacent = new List<(int x, int y)>();

                    foreach ((int dx, int dy) in AdjacentDirections)
                    {
                        (int nx, int ny) = (x + dx, y + dy);
                        if (IsValid(nx, ny))
                            adjacent.Add((nx, ny));
                    }
                    // An open acre will become filled with trees if three or more adjacent acres contained trees.
                    if (State[y, x] == '.' && adjacent.Where(p => State[p.y, p.x] == '|').Count() >= 3)
                        tempMap[y, x] = '|';
                    // An acre filled with trees will become a lumberyard if three or more adjacent acres were lumberyards.
                    else if (State[y, x] == '|' && adjacent.Where(p => State[p.y, p.x] == '#').Count() >= 3)
                        tempMap[y, x] = '#';
                    // An lumberyard will become open if there is not at least one lumberyard and one acre of trees
                    else if (State[y, x] == '#' && (!adjacent.Exists(p => State[p.y, p.x] == '|') || !adjacent.Exists(p => State[p.y, p.x] == '#')))
                        tempMap[y, x] = '.';
                    // No changes needed
                    else
                        tempMap[y, x] = State[y, x];
                }
            }
            State = tempMap;
            Minute++;
        }

        public bool IsValid(int x, int y)
        {
            return (0 <= x && x < State.GetLength(1) && 0 <= y && y < State.GetLength(0));
        }

        public void PrintOut()
        {
            using (var sw = new StreamWriter(string.Format("Day18Map{0}.txt", Minute)))
            {
                for (int y = 0; y < State.GetLength(0); ++y)
                {
                    for (int x = 0; x < State.GetLength(1); ++x)
                    {
                        sw.Write(State[y, x]);
                    }
                    sw.WriteLine();
                }
                sw.WriteLine("Minute {0}, Resoures {1}", Minute, CountResource());
            }
        }

        public bool Equals(ForestMap other)
        {
            return StateComparer.ContentEquals(this.State, other.State);
        }

        internal ForestMap Clone()
        {
            var cloneMap = new ForestMap
            {
                State = (char[,])State.Clone(),
                OriginalState = (char[,])State.Clone()
            };

            return cloneMap;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < State.GetLength(0); ++y)
            {
                for (int x = 0; x < State.GetLength(1); ++x)
                {
                    sb.Append(State[y, x]);
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append($"Minute {Minute}, Resoures {CountResource()}");

            return sb.ToString();
        }
    }

    public static class StateComparer
    {
        public static bool ContentEquals(this char[,] ary, char[,] other)
        {
            if (ary.GetLength(0) != other.GetLength(0) || ary.GetLength(1) != other.GetLength(1))
                return false;
            for (int i = 0; i < ary.GetLength(0); ++i)
            {
                for (int j = 0; j < other.GetLength(1); ++j)
                {
                    if (ary[i, j] != other[i, j])
                        return false;
                }
            }
            return true;
        }

    }
}
