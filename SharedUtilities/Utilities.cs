using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedUtilities
{
    public class Utilities
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
    }
}
