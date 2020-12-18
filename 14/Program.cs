using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _14 {
    static class Program {
        static async Task Main(string[] args) {
            var instructions = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Sum the values in memory after using a bitmask on binary values
             */
            var memory = new Dictionary<int, char[]>();
            var mask = new char[36];
            var sum = (long)0;

            foreach (var instruction in instructions) {
                if (instruction.StartsWith("mem")) {
                    var instruction_parts = instruction.Split(" = ");
                    var address = Convert.ToInt32(instruction_parts[0].Replace("mem[", string.Empty).Replace("]", string.Empty));
                    var value = GetBinaryValue(Convert.ToInt32(instruction_parts[1]));

                    if (memory.ContainsKey(address) == false) {
                        memory[address] = new char[36];
                    }

                    for (var i = 0; i < value.Length; i++) {
                        memory[address][i] = mask[i] == 'X' ? value[i] : mask[i];
                    }
                } else {
                    // update the bitmask, storing in little endian format (least significant bit first)
                    mask = instruction.Replace("mask = ", string.Empty).Reverse().ToArray();
                }
            }

            foreach (var block in memory) {
                sum += GetDecimalValue(block.Value);
            }

            Console.WriteLine(sum);

            /*
             * Part 2
             *
             * Sum the values in memory after using a bitmask on memory addresses
             */
            var big_memory = new Dictionary<long, char[]>();
            mask = new char[36];
            sum = (long)0;

            foreach (var instruction in instructions) {
                if (instruction.StartsWith("mem")) {
                    var instruction_parts = instruction.Split(" = ");
                    var address = Convert.ToInt32(instruction_parts[0].Replace("mem[", string.Empty).Replace("]", string.Empty));
                    var value = GetBinaryValue(Convert.ToInt32(instruction_parts[1]));
                    var addresses = GetDecodedAddresses(address, mask);

                    foreach (var decoded_address in addresses) {
                        big_memory[decoded_address] = value;
                    }
                } else {
                    // update the bitmask, storing in little endian format (least significant bit first)
                    mask = instruction.Replace("mask = ", string.Empty).Reverse().ToArray();
                }
            }

            foreach (var block in big_memory) {
                sum += GetDecimalValue(block.Value);
            }

            Console.WriteLine(sum);
        }

        // get the (little endian format) binary value of an integer
        static char[] GetBinaryValue(int input) {
            var output = new char[36];

            for (var i = 0; i < output.Length; i++) {
                output[i] = '0';
            }

            while (input != 0) {
                var lg = (int)Math.Floor(Math.Log2(input));

                output[lg] = '1';

                input -= (int)Math.Pow(2, lg);
            }

            return output;
        }

        // get the integer value of a binary string in little endian format
        static long GetDecimalValue(char[] input) {
            var output = (long)0;

            for (var i = 0; i < input.Length; i++) {
                if (input[i] == '1') {
                    output += (long)Math.Pow(2, i);
                }
            }

            return output;
        }

        static List<long> GetDecodedAddresses(int input, char[] mask) {
            var addresses = new List<long>();
            var binary_address = GetBinaryValue(input);
            var masked_address = new char[36];

            for (var i = 0; i < binary_address.Length; i++) {
                masked_address[i] = mask[i] == '0' ? binary_address[i] : mask[i];
            }

            var floating_bits = masked_address.Count(bit => bit == 'X');
            var floating_bit_combinations = (int)Math.Pow(2, floating_bits);

            for (var i = 0; i < floating_bit_combinations; i++) {
                var bit_replacement = GetBinaryValue(i);
                var temp_masked_address = masked_address.ToArray();
                
                for (var j = 0; j < floating_bits; j++) {
                    temp_masked_address[Array.IndexOf(temp_masked_address, 'X')] = bit_replacement[j];
                }

                addresses.Add(GetDecimalValue(temp_masked_address));
            }

            return addresses;
        }
    }
}