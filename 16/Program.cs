using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _16 {
    class Program {
        static async Task Main(string[] args) {
            var ticket_data = await File.ReadAllTextAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Find the ticket scanning error rate
             */
            var ticket_data_parts = ticket_data.Split("\n\n");
            var ticket_fields = ticket_data_parts[0].Split('\n');
            var nearby_tickets = ticket_data_parts[2].Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

            // remove the label row
            nearby_tickets.RemoveAt(0);

            var ticket_field_ranges = ticket_fields.Select(field => field.Split(':')[1])
                                                   .Select(field => (range1: field.Split(" or ")[0], range2: field.Split(" or ")[1]))
                                                   .Select(field => (range1: (lower: Convert.ToInt32(field.range1.Split('-')[0]), upper: Convert.ToInt32(field.range1.Split('-')[1])), range2: (lower: Convert.ToInt32(field.range2.Split('-')[0]), upper: Convert.ToInt32(field.range2.Split('-')[1]))));

            var valid_ticket_field_values = new HashSet<int>();

            foreach (var range in ticket_field_ranges) {
                for (var i = range.range1.lower; i <= range.range1.upper; i++) {
                    valid_ticket_field_values.Add(i);
                }

                for (var i = range.range2.lower; i <= range.range2.upper; i++) {
                    valid_ticket_field_values.Add(i);
                }
            }

            var invalid_nearby_ticket_values = nearby_tickets.Select(ticket => ticket.Split(",").Select(val => Convert.ToInt32(val)).Where(val => valid_ticket_field_values.Contains(val) == false));
            var ticket_scanning_error_rate = 0;

            foreach (var invalid_value_list in invalid_nearby_ticket_values) {
                ticket_scanning_error_rate += invalid_value_list.Sum();
            }

            Console.WriteLine(ticket_scanning_error_rate);

            /*
             * Part 2
             *
             * Find your 6 ticket values for "departure" fields
             */
            var valid_tickets = nearby_tickets.Select(ticket => ticket.Split(",").Select(val => Convert.ToInt32(val)).Where(val => valid_ticket_field_values.Contains(val)).ToList())
                                              .Where(ticket => ticket.Count() == nearby_tickets[0].Count(ch => ch == ',') + 1)
                                              .ToList();

            var ticket_fields_and_ranges = ticket_fields.Select(field => (label: field.Split(':')[0], range: field.Split(':')[1]))
                                                        .Select(field => (label: field.label, range1: field.range.Split(" or ")[0], range2: field.range.Split(" or ")[1]))
                                                        .Select(field => (label: field.label, range1: (lower: Convert.ToInt32(field.range1.Split('-')[0]), upper: Convert.ToInt32(field.range1.Split('-')[1])), range2: (lower: Convert.ToInt32(field.range2.Split('-')[0]), upper: Convert.ToInt32(field.range2.Split('-')[1]))))
                                                        .ToList();

            var possible_field_indexes = new Dictionary<string, HashSet<int>>();

            foreach (var field_and_range in ticket_fields_and_ranges) {
                for (var i = 0; i < valid_tickets[0].Count; i++) {
                    if (valid_tickets.All(valid_ticket => (valid_ticket[i] >= field_and_range.range1.lower && valid_ticket[i] <= field_and_range.range1.upper) || (valid_ticket[i] >= field_and_range.range2.lower && valid_ticket[i] <= field_and_range.range2.upper))) {
                        if (possible_field_indexes.ContainsKey(field_and_range.label)) {
                            possible_field_indexes[field_and_range.label].Add(i);
                        } else {
                            possible_field_indexes.Add(field_and_range.label, new HashSet<int>() { i });
                        }
                    }
                }
            }

            var resolved_field_positions = false;

            while (!resolved_field_positions) {
                // reduce possibilities by ruling out values that are the only value for a particular label
                foreach (var label in possible_field_indexes.Keys) {
                    if (possible_field_indexes[label].Count == 1) {
                        foreach (var other_label in possible_field_indexes.Keys.Where(key => key != label)) {
                            possible_field_indexes[other_label].Remove(possible_field_indexes[label].First());
                        }
                    }
                }

                // reduce possibilities by finding values that are only assigned to 1 label
                foreach (var label in possible_field_indexes.Keys) {
                    foreach (var val in possible_field_indexes[label]) {
                        if (possible_field_indexes.Count(field => field.Value.Contains(val)) == 1) {
                            possible_field_indexes[label].Clear();
                            possible_field_indexes[label].Add(val);
                            break;
                        }
                    }
                }

                resolved_field_positions = !possible_field_indexes.Any(field => field.Value.Count > 1);
            }

            var your_ticket_values = ticket_data_parts[1].Replace("your ticket:\n", string.Empty).Split(',').Select(val => Convert.ToInt32(val)).ToList();
            var multiplied_departure_values = (long)1;

            foreach (var departure_field in possible_field_indexes.Where(field => field.Key.StartsWith("departure"))) {
                multiplied_departure_values *= your_ticket_values[departure_field.Value.First()];
            }

            Console.WriteLine(multiplied_departure_values);
        }
    }
}