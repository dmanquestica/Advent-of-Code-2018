using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            var orderedList = list.OrderBy(o => o).ToList();

            var regex = new Regex(@"(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2})");

            var guardList = new HashSet<Guard>();

            Console.WriteLine("Part 1");

            Guard currentGuard = null;

            foreach (string s in orderedList)
            {
                var matches = regex.Match(s);

                if (s.Contains("Guard"))
                {
                    var splitString = s.Split(' ');
                    currentGuard = new Guard(splitString[3]);
                    guardList.Add(currentGuard);
                }
                GroupCollection groups = matches.Groups;
                var date = new DateTime(Int32.Parse(groups[1].Value),
                    Int32.Parse(groups[2].Value),
                    Int32.Parse(groups[3].Value),
                    Int32.Parse(groups[4].Value),
                    Int32.Parse(groups[5].Value), 0);

                if (currentGuard != null)
                    currentGuard.AddSchedule(date.Hour != 0 ? new DateTime(date.Year, date.Month, date.AddDays(1).Day, 0, 0, 0) :
                        date, (s.Contains("wakes") || s.Contains("begins")) ? true : false);
            }

            foreach (Guard g in guardList)
            {
                var timeAsleep = 0;
                for (int i = 1; i < g.Schedules.Count; ++i)
                {
                    if (g.Schedules[i - 1].Awake == false && g.Schedules[i].Awake == true)
                        timeAsleep += (int)(g.Schedules[i].Time.Subtract(g.Schedules[i - 1].Time).TotalMinutes);
                }
                g.MinutesAsleep = timeAsleep;
            }

            var sumGuardList = guardList
                .GroupBy(g => g.ID)
                .Select(grp => new string[]
                {
                    grp.First().ID,
                    grp.Sum(g => g.MinutesAsleep).ToString()
                }
                ).OrderByDescending(s => s[1]).ToList();


            var maxSleepingGuard = sumGuardList.First();

            Console.WriteLine(int.Parse(maxSleepingGuard[0].Replace("#", string.Empty)) * FindMinuteSpentMostAsleep(maxSleepingGuard[0], guardList));

            Console.WriteLine("Part 2");

            var groupedGuardList = new Dictionary<string, Tuple<int, int>>();

            var uniqueIDs = guardList.Select(g => g.ID).Distinct();

            Tuple<int, int> current = Tuple.Create(0, 0);

            foreach (var id in uniqueIDs)
            {
                current = FindNumberOfTimesSpentAsleep(guardList.Where(g => g.ID == id).ToList());
                if (!groupedGuardList.ContainsKey(id))
                    groupedGuardList.Add(id, current);
            }

            Console.WriteLine(int.Parse(groupedGuardList.First().Key.Replace("#", string.Empty)) * groupedGuardList.First().Value.Item2);

            Console.ReadKey();
        }

        public static Tuple<int, int> FindNumberOfTimesSpentAsleep(List<Guard> guardList)
        {
            int[] minuteMostOftenAsleep = new int[60];

            foreach (var guard in guardList) {
                for (int i = 1; i < guard.Schedules.Count; ++i)
                {
                    for (int j = 0; j < minuteMostOftenAsleep.Length; ++j)
                        if ((guard.Schedules[i - 1].Awake == false && guard.Schedules[i].Awake == true)
                                && (guard.Schedules[i - 1].Time.Minute <= j && j < guard.Schedules[i].Time.Minute))
                            minuteMostOftenAsleep[j] += 1;
                }
            }
            return Tuple.Create(minuteMostOftenAsleep.Max(),
                minuteMostOftenAsleep.ToList().IndexOf(minuteMostOftenAsleep.Max()));
        }

        public static int FindMinuteSpentMostAsleep(string id, HashSet<Guard> guardList)
        {
            int[] minuteMostOftenAsleep = new int[60];

            foreach (var guard in guardList.Where(g => g.ID == id))
            {
                for (int i = 1; i < guard.Schedules.Count; ++i)
                {
                    for (int j = 0; j < minuteMostOftenAsleep.Length; ++j)
                        if ((guard.Schedules[i - 1].Awake == false && guard.Schedules[i].Awake == true)
                                && (guard.Schedules[i - 1].Time.Minute <= j && j < guard.Schedules[i].Time.Minute))
                            minuteMostOftenAsleep[j] += 1;
                }
            }
            return minuteMostOftenAsleep.ToList().IndexOf(minuteMostOftenAsleep.Max());
        }

        public static void WriteOut(List<Guard> guardList)
        {
            using (StreamWriter sw = new StreamWriter("Day4Result.txt"))
            {
                foreach (Guard g in guardList)
                {
                    sw.Write(string.Format("{0}\t", g.ID));

                    int[] array = new int[60];
                    foreach (Schedule s in g.Schedules)
                    {
                        for (int i = 0; i < array.Length; ++i)
                        {
                            if (s.Time.Minute <= i)
                            {
                                if (s.Awake == true)
                                    array[i] = 1;
                                else
                                    array[i] = 0;
                            }
                        }
                    }

                    for (int i = 0; i < array.Length; ++i)
                    {
                        sw.Write(array[i] == 1 ? "." : "*");
                    }
                    sw.WriteLine();
                }
            }
        }
    }


    public class Schedule
    {
        public bool Awake { get; set; }
        public DateTime Time { get; set; }

        public Schedule(DateTime time, bool awake)
        {
            Awake = awake;
            Time = time;
        }
    }

    public class Guard
    {
        public string ID { get; set; }
        public List<Schedule> Schedules { get; set; }
        public int MinutesAsleep { get; set; }

        public Guard(string id)
        {
            ID = id;
            Schedules = new List<Schedule>();
        }

        public void AddSchedule(DateTime datetime, bool awake)
        {
            Schedules.Add(new Schedule(datetime, awake));
        }
    }
}
