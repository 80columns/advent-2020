using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _13 {
    class Program {
        static async Task Main(string[] args) {
            var bus_schedule = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Find the earliest bus you can take to the airport & how long you have to wait for this bus
             */
            var bus_id = 0;
            var wait_minutes = int.MaxValue;

            var earliest_timestamp = Convert.ToInt32(bus_schedule[0]);
            var bus_departures = bus_schedule[1].Split(",").Where(schedule => schedule != "x").Select(schedule => Convert.ToInt32(schedule));

            foreach (var departure in bus_departures) {
                var mod = earliest_timestamp % departure;

                if (mod == 0) {
                    wait_minutes = 0;
                    bus_id = departure;
                } else {
                    var temp_wait_minutes = (earliest_timestamp - mod + departure) - earliest_timestamp;

                    if (temp_wait_minutes < wait_minutes) {
                        wait_minutes = temp_wait_minutes;
                        bus_id = departure;
                    }
                }
            }

            Console.WriteLine(wait_minutes * bus_id);

            /*
             * Part 2
             *
             * Find the earliest timestamp such that all of the listed bus IDs depart at offsets matching their positions in the list
             */
            var all_bus_departures = bus_schedule[1].Split(",").Select((schedule, offset) => schedule != "x" ? (value: Convert.ToInt64(schedule), offset) : (value: null as long?, offset)).Where(schedule => schedule.value.HasValue).Select(schedule => (schedule.value.Value, schedule.offset)).ToList();
            var timestamp_multiplier = 0;
            var departure_index = 1;
            var search_value = all_bus_departures[0].Value;
            var base_offset = (long)0;

            while (true) {
                timestamp_multiplier++;

                var starting_timestamp = base_offset + (timestamp_multiplier * search_value);

                if ((starting_timestamp + all_bus_departures[departure_index].offset) % all_bus_departures[departure_index].Value == 0) {
                    if (departure_index + 1 == all_bus_departures.Count) {
                        Console.WriteLine(starting_timestamp);
                        break;
                    } else {
                        base_offset = starting_timestamp;
                        timestamp_multiplier = 0;
                        search_value *= all_bus_departures[departure_index].Value;
                        departure_index++;
                    }
                }
            }
        }
    }
}