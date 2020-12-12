using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8 {
    static class Program {
        static async Task Main(string[] args) {
            var instructions = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Find the value of the accumulator just before any instruction is executed a 2nd time
             *
             *
             * Part 2
             *
             * Fix the input so it executes to the end of the instructions,
             * and find the value of the accumulator when the instructions
             * have finished executing
             */
            var printed_part_1_solution = false;
            var next_instruction_index = 0;
            var temp_changed_instruction = ((index: -1, instruction: ""), visited_index: -1);
            var last_changed_instruction_index = instructions.Length;

            while (next_instruction_index < instructions.Length) {
                next_instruction_index = 0;

                var accumulator = 0;
                var visited_instructions = new SortedSet<(int index, string instruction)>();

                while (
                    visited_instructions.Any(x => x.index == next_instruction_index) == false
                 && next_instruction_index != instructions.Length
                ) {
                    visited_instructions.Add((next_instruction_index, instructions[next_instruction_index]));

                    var instruction_parts = instructions[next_instruction_index].Split(" ");

                    if (instruction_parts[0] == "acc") {
                        if (instruction_parts[1].Contains("+")) {
                            accumulator += Convert.ToInt32(instruction_parts[1].Replace("+", string.Empty));
                        } else {
                            accumulator -= Convert.ToInt32(instruction_parts[1].Replace("-", string.Empty));
                        }

                        next_instruction_index++;
                    } else if (instruction_parts[0] == "jmp") {
                        if (instruction_parts[1].Contains("+")) {
                            next_instruction_index += Convert.ToInt32(instruction_parts[1].Replace("+", string.Empty));
                        } else {
                            next_instruction_index -= Convert.ToInt32(instruction_parts[1].Replace("-", string.Empty));
                        }
                    } else {
                        next_instruction_index++;
                    }
                }

                if (
                    printed_part_1_solution == false
                 && next_instruction_index != instructions.Length
                ) {
                    Console.WriteLine(accumulator);
                    printed_part_1_solution = true;
                }

                if (next_instruction_index != instructions.Length) {
                    if (temp_changed_instruction.Item1.index != -1) {
                        // restore the previous temp changed instruction
                        instructions[temp_changed_instruction.Item1.index] = temp_changed_instruction.Item1.instruction;
                        last_changed_instruction_index = temp_changed_instruction.visited_index;

                        // reset the temp changed instruction so we do a re-run of the original
                        // instruction set before changing a different instruction
                        temp_changed_instruction = ((index: -1, instruction: ""), visited_index: -1);
                    } else {
                        temp_changed_instruction = visited_instructions.Select((x, j) => (x, j)).Where(
                        (x, i) => ((x.x.instruction.StartsWith("jmp") || x.x.instruction.StartsWith("nop")))
                                  && i < last_changed_instruction_index
                        ).Last();

                        if (temp_changed_instruction.Item1.instruction.StartsWith("jmp")) {
                            instructions[temp_changed_instruction.Item1.index] = instructions[temp_changed_instruction.Item1.index].Replace("jmp", "nop");
                        } else {
                            instructions[temp_changed_instruction.Item1.index] = instructions[temp_changed_instruction.Item1.index].Replace("nop", "jmp");
                        }
                    }
                } else {
                    Console.WriteLine(accumulator);
                }
            }
        }
    }
}