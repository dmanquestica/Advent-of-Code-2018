using SharedUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day16
{
    public class Program
    {

        public static string[] OpCodes = { "addr", "addi",
                                           "mulr", "muli",
                                           "banr", "bani",
                                           "borr", "bori",
                                           "setr", "seti",
                                           "gtir", "gtri", "gtrr",
                                           "eqir", "eqri", "eqrr" };

        public static List<Part1Input> Samples = new List<Part1Input>();
        public static List<Part2Input> Instructions = new List<Part2Input>();

        public static Dictionary<int, string> MappedInstructions = new Dictionary<int, string>(0);

        public static void Main(string[] args)
        {
            Console.WriteLine("Part 1");

            var part1Input = Utilities.ReadFile(args[0]);

            ParsePart1Input(part1Input);

            foreach (var sample in Samples)
            {
                for (int i = 0; i < OpCodes.Length; ++i)
                {
                    sample.TestOpCode(OpCodes[i]);
                }
            }
            Console.WriteLine(Samples.Count(o => o.OpCodes.Count >= 3));

            Console.WriteLine("Part 2");

            // Process the possibilities
            MapInstruction();

            // Use mapping to solve Part 2
            var part2Input = Utilities.ReadFile(args[1]);
            ParsePart2Input(part2Input);

            var initialRegister = new int[] { 0, 0, 0, 0 };

            foreach (var instr in Instructions)
            {
                instr.ApplyInstruction(ref initialRegister, MappedInstructions);
            }
            Console.WriteLine("R0 : {0}", initialRegister[0]);

            Console.ReadKey();
        }

        private static void ParsePart2Input(IList<string> input)
        {
            for (int i = 0; i < input.Count; ++i)
                Instructions.Add(new Part2Input(input[i]));
        }

        public static void ParsePart1Input(IList<string> input)
        {
            for (int i = 0; i < input.Count;)
            {
                var tempPart1Input = new Part1Input();
                // Before
                var stringArray = input[i++].Replace("Before: [", string.Empty).Replace("]", string.Empty);
                tempPart1Input.Before = stringArray.Split(',').Select(int.Parse).ToArray();

                stringArray = input[i++];
                tempPart1Input.Instruction = stringArray.Split(' ').Select(int.Parse).ToArray();

                // After
                stringArray = input[i++].Replace("After:  [", string.Empty).Replace("]", string.Empty);
                tempPart1Input.After = stringArray.Split(',').Select<string, int>(int.Parse).ToArray();

                i++;

                Samples.Add(tempPart1Input);
            }
        }

        private static void MapInstruction()
        {
            var possibles = new SortedDictionary<int, List<string>>();

            foreach (var s in Samples.Distinct())
            {
                if (!possibles.ContainsKey(s.Instruction[0]))
                    possibles.Add(s.Instruction[0], s.OpCodes.ToList());
                else
                    possibles[s.Instruction[0]].AddRange(s.OpCodes.ToList().Except(possibles[s.Instruction[0]]).ToList());
            }
            do
            {
                foreach (var p in possibles.Where(o => o.Value.Count == 1).ToList())
                {
                    var opCode = p.Value[0];
                    MappedInstructions.Add(p.Key, opCode);
                    possibles.Remove(p.Key);
                    foreach (var po in possibles.ToList())
                        possibles[po.Key] = po.Value.Where(op => op != opCode).ToList();
                }
            } while (possibles.Any());
        }
    }

    public class Part2Input
    {
        public int[] Instruction { get; set; }

        public Part2Input(string instruction)
        {
            Instruction = instruction.Split(' ').Select(int.Parse).ToArray();
        }

        public void ApplyInstruction(ref int[] register, Dictionary<int, string> MappedInstructions)
        {
            switch (MappedInstructions[Instruction[0]]) {
                case "addr":
                    register[Instruction[3]] = register[Instruction[1]] + register[Instruction[2]];
                    break;
                case "addi":
                    register[Instruction[3]] = register[Instruction[1]] + Instruction[2];
                    break;
                case "mulr":
                    register[Instruction[3]] = register[Instruction[1]] * register[Instruction[2]];
                    break;
                case "muli":
                    register[Instruction[3]] = register[Instruction[1]] * Instruction[2];
                    break;
                case "banr":
                    register[Instruction[3]] = register[Instruction[1]] & register[Instruction[2]];
                    break;
                case "bani":
                    register[Instruction[3]] = register[Instruction[1]] & Instruction[2];
                    break;
                case "borr":
                    register[Instruction[3]] = register[Instruction[1]] | register[Instruction[2]];
                    break;
                case "bori":
                    register[Instruction[3]] = register[Instruction[1]] | Instruction[2];
                    break;
                case "setr":
                    register[Instruction[3]] = register[Instruction[1]];
                    break;
                case "seti":
                    register[Instruction[3]] = Instruction[1];
                    break;
                case "gtir":
                    register[Instruction[3]] = Instruction[1] > register[Instruction[2]] ? 1 : 0;
                    break;
                case "gtri":
                    register[Instruction[3]] = register[Instruction[1]] > Instruction[2] ? 1 : 0;
                    break;
                case "gtrr":
                    register[Instruction[3]] = register[Instruction[1]] > register[Instruction[2]] ? 1 : 0;
                    break;
                case "eqir":
                    register[Instruction[3]] = Instruction[1] == register[Instruction[2]] ? 1 : 0;
                    break;
                case "eqri":
                    register[Instruction[3]] = register[Instruction[1]] == Instruction[2] ? 1 : 0;
                    break;
                case "eqrr":
                    register[Instruction[3]] = register[Instruction[1]] == register[Instruction[2]] ? 1 : 0;
                    break;
                default:
                    throw new Exception("Unknown instruction!");
            }
        }
    }

    public class Part1Input
    {
        public int[] Before { get; set; }
        public int[] Instruction { get; set; }
        public int[] After { get; set; }

        public SortedSet<string> OpCodes { get; set; }

        public Part1Input()
        {
            Before = new int[4];
            Instruction = new int[4];
            After = new int[4];
            OpCodes = new SortedSet<string>();
        }

        public void TestOpCode(string opCode)
        {
            var resultRegister = (int[])Before.Clone();

            if (opCode.Equals("addr"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] + Before[Instruction[2]];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("addi"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] + Instruction[2];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("mulr"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] * Before[Instruction[2]];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("muli"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] * Instruction[2];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("banr"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] & Before[Instruction[2]];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("bani"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] & Instruction[2];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("borr"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] | Before[Instruction[2]];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("bori"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] | Instruction[2];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("setr"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("seti"))
            {
                resultRegister[Instruction[3]] = Instruction[1];
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("gtir"))
            {
                resultRegister[Instruction[3]] = Instruction[1] > Before[Instruction[2]] ? 1 : 0;
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("gtri"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] > Instruction[2] ? 1 : 0;
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("gtrr"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] > Before[Instruction[2]] ? 1 : 0;
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("eqir"))
            {
                resultRegister[Instruction[3]] = Instruction[1] == Before[Instruction[2]] ? 1 : 0;
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("eqri"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] == Instruction[2] ? 1 : 0;
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
            if (opCode.Equals("eqrr"))
            {
                resultRegister[Instruction[3]] = Before[Instruction[1]] == Before[Instruction[2]] ? 1 : 0;
                if (Enumerable.SequenceEqual(resultRegister, After))
                    OpCodes.Add(opCode);
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append(Instruction[0] + ": ");
            result.Append(string.Join(", ", OpCodes.ToArray()));

            return result.ToString();
        }
    }

}
