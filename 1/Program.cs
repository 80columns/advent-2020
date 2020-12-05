using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1 {
    class Program {
        static async Task Main(string[] args) {
            var set = new HashSet<int>((await File.ReadAllTextAsync("input.txt", Encoding.UTF8)).Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)));
            var lowers = set.Where(x => x < (2020 / 2)).ToHashSet<int>();

            /*
             * Part 1
             *
             * Pair sum
             */
            Console.WriteLine(
                new int[] {
                    set.First(item => lowers.Contains(2020 - item))
                }
                .Select(item => item * (2020 - item))
                .First()
            );

            /*
             * Part 2
             *
             * Triplet sum
             */
            Console.WriteLine(
                new (int first_lower, int second_lower)[] {
                    lowers.SelectMany(a => lowers, (first_lower, second_lower) => (first_lower, second_lower))
                          .First(pair => set.Contains(2020 - pair.first_lower - pair.second_lower))
                }
                .Select(z => z.first_lower * z.second_lower * (2020 - z.first_lower - z.second_lower))
                .First()
            );
        }
    }
}