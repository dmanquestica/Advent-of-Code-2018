using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day21
{
    public class Program
    {
        public static int InstructionPointer { get; set; }
        public static List<(string opCode, int[] values)> InstructionList { get; set; }

        public static int[] Registers = new int[] { 0, 0, 0, 0, 0, 0 };

        public static void Main(string[] args)
        {
            var input = Utilities.ReadFile(args[0]);

            ParseInput(input);

            Console.WriteLine("Part 1");

            var opCodeToCheck = InstructionList.Where(i => i.opCode == "eqrr").First();
            var index = InstructionList.IndexOf(opCodeToCheck);
            var registerToCheck = opCodeToCheck.values[0];

            while (true)
            {
                ApplyInstruction(InstructionList[Registers[InstructionPointer]]);
                if (Registers[InstructionPointer] == index) {
                    Console.WriteLine(Registers[registerToCheck]);
                    break;
                }
            }

            Console.WriteLine("Part 2");

            // Reset Registers
            Registers = new int[] { 0, 0, 0, 0, 0, 0 };

            var Seen = new List<int>();
            var Previous = 0;

            while (InstructionPointer < InstructionList.Count())
            {
                ApplyInstruction(InstructionList[Registers[InstructionPointer]]);
                if (Registers[InstructionPointer] == index)
                {
                    if (Seen.Contains(Registers[registerToCheck]))
                    {
                        Console.WriteLine(Previous);
                        break;
                    }
                    Seen.Add(Registers[registerToCheck]);
                    Previous = Registers[registerToCheck];
                }
            }

            Console.ReadKey();
        }

        public static void ParseInput(IList<string> lines)
        {
            // Input parsing
            InstructionPointer = Int32.Parse(lines[0].Split(' ')[1]);
            InstructionList = new List<(string, int[])>();
            for (int i = 1; i < lines.Count; ++i)
            {
                var pattern = @"^(\w+) (\d+) (\d+) (\d+)$";
                var regex = new Regex(pattern);
                var m = regex.Match(lines[i]);
                InstructionList.Add((m.Groups[1].Value, new int[] {
                    Int32.Parse(m.Groups[2].Value),
                    Int32.Parse(m.Groups[3].Value),
                    Int32.Parse(m.Groups[4].Value) }));
            }
        }

        public static void ApplyInstruction((string opCode, int[] values) Instruction)
        {
            switch (Instruction.opCode)
            {
                case "addr":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] + Registers[Instruction.values[1]];
                    break;
                case "addi":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] + Instruction.values[1];
                    break;
                case "mulr":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] * Registers[Instruction.values[1]];
                    break;
                case "muli":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] * Instruction.values[1];
                    break;
                case "banr":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] & Registers[Instruction.values[1]];
                    break;
                case "bani":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] & Instruction.values[1];
                    break;
                case "borr":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] | Registers[Instruction.values[1]];
                    break;
                case "bori":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] | Instruction.values[1];
                    break;
                case "setr":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]];
                    break;
                case "seti":
                    Registers[Instruction.values[2]] = Instruction.values[0];
                    break;
                case "gtir":
                    Registers[Instruction.values[2]] = Instruction.values[0] > Registers[Instruction.values[1]] ? 1 : 0;
                    break;
                case "gtri":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] > Instruction.values[1] ? 1 : 0;
                    break;
                case "gtrr":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] > Registers[Instruction.values[1]] ? 1 : 0;
                    break;
                case "eqir":
                    Registers[Instruction.values[2]] = Instruction.values[0] == Registers[Instruction.values[1]] ? 1 : 0;
                    break;
                case "eqri":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] == Instruction.values[1] ? 1 : 0;
                    break;
                case "eqrr":
                    Registers[Instruction.values[2]] = Registers[Instruction.values[0]] == Registers[Instruction.values[1]] ? 1 : 0;
                    break;
                default:
                    throw new Exception("Unknown instruction!");
            }
            Registers[InstructionPointer]++;
        }
    }
}
