using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9 {
    class Program {
        static async Task Main(string[] args) {
            var numbers = (await File.ReadAllLinesAsync("input.txt", Encoding.UTF8)).Select(line => Convert.ToInt64(line)).ToList();
            var invalid_index = -1;

            /*
             * Part 1
             *
             * Find the invalid number in the list, where 2 numbers in the previous
             * 25 numbers cannot be added together to get the value of that number
             */
            for (var i = 25; i < numbers.Count; i++) {
                var previous_25 = numbers.Where((x, j) => j >= i - 25 && j < i ).ToHashSet();
                var found_sum = false;    

                foreach (var item in previous_25) {
                    if (previous_25.Contains(numbers[i] - item) == true) {
                        found_sum = true;
                        break;
                    }
                }

                if (found_sum == false) {
                    invalid_index = i;
                    break;
                }
            }

            Console.WriteLine(numbers[invalid_index]);

            /*
             * Part 2
             *
             * Find the largest & smallest numbers in the continuous sequence
             * of numbers that adds up to the invalid number found previously
             */
            var temp_sum = (long)0;
            var temp_sum_start_index = 0;
            var smallest_range_number = Int64.MaxValue;
            var largest_range_number = Int64.MinValue;

            for (var i = 0; i < numbers.Count; i++) {
                if (temp_sum < numbers[invalid_index]) {
                    temp_sum += numbers[i];

                    if (numbers[i] < smallest_range_number) {
                        smallest_range_number = numbers[i];
                    } else if (numbers[i] > largest_range_number) {
                        largest_range_number = numbers[i];
                    }
                } else if (temp_sum == numbers[invalid_index]) {
                    Console.WriteLine(smallest_range_number + largest_range_number);

                    break;
                } else {
                    // reset i to start from the next number
                    i = temp_sum_start_index;
                    temp_sum = 0;
                    temp_sum_start_index++;

                    smallest_range_number = Int64.MaxValue;
                    largest_range_number = Int64.MinValue;
                }
            }
        }
    }
}