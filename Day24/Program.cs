using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day24
{
    public class Program
    {
        public static HashSet<Group> Armies;

        public static void Main(string[] args)
        {
            var list = Utilities.ReadFile(args[0]);

            Console.WriteLine("Part 1");

            InitializeGroups(list);

            var result = Fight();

            Console.WriteLine($"Group {(result.ImmuneWin ? "Immune System" : "Infection")} wins with { result.units} left");

            Console.WriteLine("Part 2");

            var minBoost = 0;

            do
            {
                InitializeGroups(list);
                minBoost++;
            } while (!Fight(minBoost).ImmuneWin);

            result = Fight(minBoost);

            Console.WriteLine($"Minimum Boost required: {minBoost}");

            Console.WriteLine($"Group {(result.ImmuneWin ? "Immune System" : "Infection")} wins with {result.units} left");

            Console.ReadKey();

        }

        public static (bool ImmuneWin, int units) Fight(int boost = 0)
        {
            foreach (var group in Armies.Where(g => g.GroupType == GroupType.ImmuneSystem))
                group.Damage += boost;

            var continueAttack = true;
            while (continueAttack)
            {
                continueAttack = false;
                var remaining = new HashSet<Group>(Armies);
                var targets = new Dictionary<Group, Group>();
                foreach (var g in Armies.OrderByDescending(g => (g.EffectivePower, g.Initiative)))
                {
                    var maxDamage = remaining.Select(t => g.AttackDamage(t)).Max();
                    if (maxDamage > 0)
                    {
                        var possibleTargets = remaining.Where(t => g.AttackDamage(t) == maxDamage);
                        targets[g] = possibleTargets.OrderByDescending(t => (t.EffectivePower, t.Initiative)).First();
                        remaining.Remove(targets[g]);
                    }
                }
                foreach (var g in targets.Keys.OrderByDescending(g => g.Initiative))
                {
                    if (g.Units > 0)
                    {
                        var target = targets[g];
                        var damage = g.AttackDamage(target);
                        if (damage > 0 && target.Units > 0)
                        {
                            var dies = damage / target.HP;
                            target.Units = Math.Max(0, target.Units - dies);
                            if (dies > 0)
                                continueAttack = true;
                        }
                    }
                }
                Armies = Armies.Where(g => g.Units > 0).ToHashSet();
            }
            return (Armies.All(x => x.GroupType == GroupType.ImmuneSystem), Armies.Select(x => x.Units).Sum());

        }

        public static void InitializeGroups(IList<string> list)
        {
            Armies = new HashSet<Group>();

            var groupType = GroupType.ImmuneSystem;

            foreach (var line in list)
            {
                if (line.Length > 0)
                {
                    if (line.Trim().Equals("Immune System:"))
                    {
                        groupType = GroupType.ImmuneSystem;
                        continue;
                    }
                    else if (line.Trim().Equals("Infection:"))
                    {
                        groupType = GroupType.Infection;
                        continue;
                    }
                    Armies.Add(new Group(line, groupType));
                }
            }
        }
    }

    public enum GroupType
    {
        ImmuneSystem,
        Infection
    }

    public enum AttackType
    {
        Bludgeoning,
        Radiation,
        Cold,
        Fire,
        Slashing
    }

    public class Group
    {
        public GroupType GroupType { get; set; }
        public int Units { get; set; }
        public int HP { get; set; }
        public int Damage { get; set; }
        public int Initiative { get; set; }
        public AttackType AttackType { get; set; }
        public HashSet<AttackType> Immunity = new HashSet<AttackType>();
        public HashSet<AttackType> Weakness = new HashSet<AttackType>();

        public int EffectivePower
        {
            get
            {
                return Units * Damage;
            }
        }

        public int AttackDamage(Group target)
        {
            if (this.GroupType == target.GroupType)
                return 0;
            else if (target.Immunity.Contains(this.AttackType))
                return 0;
            else if (target.Weakness.Contains(this.AttackType))
                return EffectivePower * 2;
            else
                return EffectivePower;
        }

        public Group()
        {

        }

        public Group(string input, GroupType groupType)
        {
            GroupType = groupType;

            var pattern = @"(\d+) units each with (\d+) hit points(.*)with an attack that does (\d+)(.*)damage at initiative (\d+)";
            var m = Regex.Match(input, pattern);
            if (m.Success)
            {
                Units = int.Parse(m.Groups[1].Value);
                HP = int.Parse(m.Groups[2].Value);
                Damage = int.Parse(m.Groups[4].Value);
                AttackType = (AttackType)Enum.Parse(typeof(AttackType), m.Groups[5].Value.Trim(), true);
                Initiative = int.Parse(m.Groups[6].Value);
                var st = m.Groups[3].Value.Trim();
                if (st != "")
                {
                    st = st.Substring(1, st.Length - 2);
                    foreach (var part in st.Split(';'))
                    {
                        var k = part.Split(new string[] { " to " }, StringSplitOptions.None);
                        var set = (k[1].Split(new string[] { ", " }, StringSplitOptions.None)).ToList();
                        var w = k[0].Trim();
                        if (w == "immune")
                        {
                            foreach (var s in set)
                                Immunity.Add((AttackType)Enum.Parse(typeof(AttackType), s, true));
                        }
                        else if (w == "weak")
                        {
                            foreach (var s in set)
                                Weakness.Add((AttackType)Enum.Parse(typeof(AttackType), s, true));
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
        }
    }
}
