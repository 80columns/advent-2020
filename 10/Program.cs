using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10 {
    class Program {
        static async Task Main(string[] args) {
            var adapter_joltages = (await File.ReadAllLinesAsync("input.txt", Encoding.UTF8)).Select(item => Convert.ToInt32(item)).ToList();
            var difference_one_count = 0;
            var difference_three_count = 0;
            var last_joltage = 0;

            adapter_joltages.Sort();

            for (var i = 0; i < adapter_joltages.Count; i++) {
                if (adapter_joltages[i] - last_joltage == 1) {
                    difference_one_count++;
                } else if (adapter_joltages[i] - last_joltage == 3) {
                    difference_three_count++;
                }

                last_joltage = adapter_joltages[i];
            }

            difference_three_count++;

            Console.WriteLine(difference_one_count * difference_three_count);

            adapter_joltages.Sort();
            var joltage_arrangements = new Dictionary<long, long>();

            foreach (var adapter_jolt in adapter_joltages) {
                if (adapter_jolt >= 4) {
                    joltage_arrangements[adapter_jolt] = (joltage_arrangements.ContainsKey(adapter_jolt - 1) ? joltage_arrangements[adapter_jolt - 1] : 0)
                                                       + (joltage_arrangements.ContainsKey(adapter_jolt - 2) ? joltage_arrangements[adapter_jolt - 2] : 0)
                                                       + (joltage_arrangements.ContainsKey(adapter_jolt - 3) ? joltage_arrangements[adapter_jolt - 3] : 0);
                } else if (adapter_jolt >= 3) {
                    joltage_arrangements[adapter_jolt] = (joltage_arrangements.ContainsKey(adapter_jolt - 1) ? joltage_arrangements[adapter_jolt - 1] : 0)
                                                       + (joltage_arrangements.ContainsKey(adapter_jolt - 2) ? joltage_arrangements[adapter_jolt - 2] : 0)
                                                       + 1;
                } else if (adapter_jolt >= 2) {
                    joltage_arrangements[adapter_jolt] = (joltage_arrangements.ContainsKey(adapter_jolt - 1) ? joltage_arrangements[adapter_jolt - 1] : 0)
                                                       + 1;
                } else {
                    joltage_arrangements[adapter_jolt] = 1;
                }
            }

            Console.WriteLine(joltage_arrangements[joltage_arrangements.Keys.Max()]);
        }
    }
}