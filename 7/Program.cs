using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7 {
    static class Program {
        static IList<KeyValuePair<string, HashSet<string>>> ExpandBagEntriesContainingShinyGold(this IList<KeyValuePair<string, HashSet<string>>> source) {
            var expansions = -1;

            while (expansions != 0) {
                expansions = 0;

                var bag_entries_containing_shinygold = new Dictionary<string, HashSet<string>>(source.Where(bag_entry => bag_entry.Value.Contains("shinygold")));
                
                for (var index = 0; index < source.Count; index++) {
                    var bag_entries_to_expand = new HashSet<string>();
                    var contained_bags = new HashSet<string>();

                    foreach (var bag_entry_with_shinygold in source[index].Value.Intersect(bag_entries_containing_shinygold.Keys)) {
                        expansions++;

                        bag_entries_to_expand.Add(bag_entry_with_shinygold);
                        contained_bags.UnionWith(bag_entries_containing_shinygold[bag_entry_with_shinygold]);
                    }

                    var temp_processed_bag_entry = source[index];

                    temp_processed_bag_entry.Value.RemoveWhere(x => bag_entries_to_expand.Contains(x));
                    temp_processed_bag_entry.Value.UnionWith(contained_bags);

                    source[index] = temp_processed_bag_entry;
                }
            }

            return source;
        }

        static int GetShinyGoldBagCapacity(this List<KeyValuePair<string, List<string>>> source) {
            var expansions = -1;
            var shiny_gold_bag_index = source.FindIndex(z => z.Key == "shinygold");
            var opened_bags = 0;

            while (expansions != 0) {
                expansions = 0;

                var bags_in_shiny_gold_bag = source[shiny_gold_bag_index].Value.ToList();
                var initial_shiny_gold_bag_entry_capacity = bags_in_shiny_gold_bag.Count;
                var bags_to_open = new List<string>();

                for (var index = 0; index < initial_shiny_gold_bag_entry_capacity; index++) {
                    var contained_bag_parts = bags_in_shiny_gold_bag[index].Split(' ');
                    var contained_bag_count = Convert.ToInt32(contained_bag_parts[0]);
                    var contained_bag_label = contained_bag_parts[1];

                    var contained_bag_entry_index = source.FindIndex(bag_entry => bag_entry.Key == contained_bag_label);

                    if (source[contained_bag_entry_index].Value.First() != "noother") {
                        expansions++;

                        foreach (var inner_contained_bag_entry in source[contained_bag_entry_index].Value) {
                            var inner_contained_bag_parts = inner_contained_bag_entry.Split(' ');
                            var inner_contained_bag_count = Convert.ToInt32(inner_contained_bag_parts[0]);

                            bags_in_shiny_gold_bag.Add($"{inner_contained_bag_count * contained_bag_count} {inner_contained_bag_parts[1]}");
                        }

                        bags_to_open.Add(bags_in_shiny_gold_bag[index]);
                    }
                }

                foreach (var bag_to_open in bags_to_open) {
                    opened_bags += Convert.ToInt32(bag_to_open.Split(' ')[0]);

                    bags_in_shiny_gold_bag.Remove(bag_to_open);
                }

                source[shiny_gold_bag_index].Value.Clear();
                source[shiny_gold_bag_index].Value.AddRange(bags_in_shiny_gold_bag);
            }

            var final_shiny_gold_bag_capacity = 0;

            foreach (var bag_in_shiny_gold_bag in source[shiny_gold_bag_index].Value) {
                var bag_in_shiny_gold_bag_count = Convert.ToInt32(bag_in_shiny_gold_bag.Split(' ')[0]);

                final_shiny_gold_bag_capacity += bag_in_shiny_gold_bag_count;
            }

            return final_shiny_gold_bag_capacity + opened_bags;
        }

        static async Task Main(string[] args) {
            var bag_entries = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Find the number of bags that could contain a shiny gold bag
             */
            Console.WriteLine(
                bag_entries.Select(z => (key: z.Split("bags contain", StringSplitOptions.None)[0].Replace(" ", string.Empty), value: Regex.Replace(Regex.Replace(z.Split("bags contain", StringSplitOptions.None)[1].TrimEnd('.'), "([0-9]*)", string.Empty), "bag(s)?", string.Empty).Replace(" ", string.Empty)))
                           .Select(z => new KeyValuePair<string, HashSet<string>>(z.key, z.value.Split(',').ToHashSet<string>())).ToList()
                           .ExpandBagEntriesContainingShinyGold()
                           .Where(z => z.Value.Contains("shinygold"))
                           .Count()
            );

            /*
             * Part 2
             *
             * Find the total number of bags that could be contained inside a shiny gold bag
             */
            Console.WriteLine(
                bag_entries.Select(z => (key: z.Split("bags contain", StringSplitOptions.None)[0].Replace(" ", string.Empty), value: Regex.Replace(z.Split("bags contain", StringSplitOptions.None)[1].TrimEnd('.'), "bag(s)?", string.Empty).Trim()))
                           .Select(z => new KeyValuePair<string, List<string>>(z.key, z.value.Split(',').Select(z => z.Trim()).Select(z => z.Substring(0, z.LastIndexOf(' ')) + z.Substring(z.LastIndexOf(' ') + 1)).ToList())).ToList()
                           .GetShinyGoldBagCapacity()
            );
        }
    }
}