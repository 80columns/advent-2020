using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2 {
    class Program {
        static async Task Main(string[] args) {
            var password_entries = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Character count password validation
             */
            Console.WriteLine(
                password_entries.Select(
                                    entry => (
                                        lower_limit: Convert.ToInt32(entry.Split(':')[0].Split(' ')[0].Split('-')[0]),
                                        upper_limit: Convert.ToInt32(entry.Split(':')[0].Split(' ')[0].Split('-')[1]),
                                        letter_count: entry.Split(':')[1].Trim().Count(password_char => password_char == entry.Split(':')[0].Split(' ')[1][0])
                                    )
                                )
                                .Count(entry => (entry.lower_limit <= entry.letter_count && entry.letter_count <= entry.upper_limit))
            );

            /*
             * Part 2
             *
             * Character position password validation
             */
            Console.WriteLine(
                password_entries.Select(
                                    entry => (
                                        lower_index_match: entry.Split(':')[1].Trim()[Convert.ToInt32(entry.Split(':')[0].Split(' ')[0].Split('-')[0]) - 1] == entry.Split(':')[0].Split(' ')[1][0],
                                        upper_index_match: entry.Split(':')[1].Trim()[Convert.ToInt32(entry.Split(':')[0].Split(' ')[0].Split('-')[1]) - 1] == entry.Split(':')[0].Split(' ')[1][0]
                                    )
                                )
                                .Count(entry => entry.lower_index_match ^ entry.upper_index_match)
            );
        }
    }
}