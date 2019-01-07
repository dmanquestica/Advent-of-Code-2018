using System;
using System.Collections.Generic;
using System.IO;

namespace SharedUtilities
{
    public static class Utilities
    {
        public static IList<string> ReadFile(string path)
        {
            var list = new List<string>();
            var file = path;

            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                        list.Add(sr.ReadLine());
                }
            }

            return list;
        }

        public static string ReadFileAsString(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                    return sr.ReadToEnd();
            }
        }

        public static int Kadane(int[] array)
        {
            int maxGlobal = int.MinValue;

            var maxCurrent = maxGlobal = array[0];
            for (int i = 1; i < array.Length; ++i)
            {
                maxCurrent = Math.Max(array[i], maxCurrent + array[i]);
                if (maxCurrent > maxGlobal)
                    maxGlobal = maxCurrent;
            }
            return maxGlobal;
        }

    }

    public class DisjointSet<T>
    {
        private static Dictionary<T, T> Parent = new Dictionary<T, T>();

        public void MakeSet(T[] universe)
        {
            foreach (var i in universe)
                Parent[i] = i;
        }

        public T Find(T k)
        {
            if (Parent[k].Equals(k))
                return k;
            return Find(Parent[k]);
        }

        public void Union(T a, T b)
        {
            var x = Find(a);
            var y = Find(b);

            Parent[x] = y;
        }

        public void Clear()
        {
            Parent = new Dictionary<T, T>();
        }

        public DisjointSet()
        {
        }
    }
}
