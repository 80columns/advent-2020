using System;
using System.Collections.Generic;
using System.Linq;

namespace _15 {
    class Program {
        static void Main(string[] args) {
            var spoken_numbers = new List<int>("0,8,15,2,12,1,4".Split(',').Select(val => Convert.ToInt32(val)));

            Func<List<int>, int, int> calculate_spoken_number = delegate(List<int> input, int ceiling) {
                // store the list of items in the Dictionary, with their indexes contained in the List to quickly get the last one
                var input_dict = new Dictionary<int, List<int>>();

                for (var i = 0; i < input.Count; i++) {
                    if (input_dict.ContainsKey(input[i])) {
                        input_dict[input[i]].Add(i);
                    } else {
                        input_dict[input[i]] = new List<int>() { i };
                    }
                }

                for (var index = input.Count - 1; index < ceiling; index++) {
                    var current = input[index];

                    if (input_dict[input[index]].Count > 1) {
                        var previous_index = input_dict[input[index]][input_dict[input[index]].Count - 2];
                        var next = index - previous_index;

                        if (input_dict.ContainsKey(next)) {
                            input_dict[next].Add(index+1);
                        } else {
                            input_dict[next] = new List<int>() { index+1 };
                        }

                        input.Add(next);
                    } else {
                        if (input_dict.ContainsKey(0)) {
                            input_dict[0].Add(index+1);
                        } else {
                            input_dict[0] = new List<int>() { index+1 };
                        }

                        input.Add(0);
                    }
                }

                return input[ceiling-1];
            };

            /*
             * Part 1
             *
             * Find the 2020th number spoken
             */
            Console.WriteLine(calculate_spoken_number(spoken_numbers, 2_020));

            /*
             * Part 2
             *
             * Find the 30000000th number spoken
             */
            Console.WriteLine(calculate_spoken_number(spoken_numbers, 30_000_000));
        }
    }
}