using SharedUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day19
{
    public class Program
    {
        public static int InstructionPointer { get; set; }
        public static List<(string opCode, int[] values)> InstructionList { get; set; }

        public static int[] Registers = new int[] { 0, 0, 0, 0, 0, 0 };
        
        public static void Main(string[] args)
        {
            var lines = Utilities.ReadFile(args[0]);

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

            File.Delete(string.Format("Day19Stream.txt"));

            Console.WriteLine("Part 1");

            while (Registers[InstructionPointer] < InstructionList.Count)
            {
                ApplyInstruction(InstructionList[Registers[InstructionPointer]]);
            }

            Console.WriteLine(Registers[0]);

            Console.WriteLine("Part 2");

            Registers = new int[] { 1, 0, 0, 0, 0, 0 };

            var iterations = 0;
            while (Registers[InstructionPointer] < InstructionList.Count)
            {
                ApplyInstruction(InstructionList[Registers[InstructionPointer]]);
                iterations++;
                StreamRegisters(iterations, InstructionList[Registers[InstructionPointer]]);
                if (iterations >= 52)
                    break;
            }

            //Console.WriteLine("After {0} Iterations: ", iterations);
            //PrintRegisters();

            // Using knowledge that R0 should hold the sum of factors of largest number 
            // when the calculations are done

            var maxValue = Registers.Max();

            var divisors = new Dictionary<int, int>() { {1, 1} };

            for (int i = 2; i <= maxValue; i++)
            {
                if (maxValue % i == 0)
                {
                    if (!divisors.ContainsKey(i))
                        divisors.Add(i, 1);
                    else
                        divisors[i] += 1;
                }
            }

            Console.WriteLine(divisors.Sum(k => k.Key ^ k.Value));

            Console.ReadKey();

        }

        public static void StreamRegisters(int iteration, (string opCode, int[] values) instruction)
        {
            using (var sw = File.AppendText(string.Format("Day19Stream.txt")))
            {
                sw.WriteLine("{0}: {1}, after applying {2} {3}", iteration, string.Join(", ", Registers), instruction.opCode, string.Join(", ", instruction.values));
            }
        }

        public static void PrintRegisters()
        {
            Console.WriteLine(string.Join(", ", Registers));
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
