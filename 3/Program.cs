using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3 {
    class Program {
        static async Task Main(string[] args) {
            var map_lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
            var line_length = map_lines[0].Length;
            
            /*
             * Part 1
             *
             * Number of trees encountered
             */
            Console.WriteLine(
                map_lines.Where((map_line, index) => map_line[(index * 3) % line_length] == '#')
                         .Count()
            );

            /*
             * Part 2
             *
             * Number of trees encountered on multiple slopes
             */
            Console.WriteLine(
                map_lines.Where((map_line, index) => map_line[(index * 1) % line_length] == '#')
                         .Count()
                *
                map_lines.Where((map_line, index) => map_line[(index * 3) % line_length] == '#')
                         .Count()
                *
                map_lines.Where((map_line, index) => map_line[(index * 5) % line_length] == '#')
                         .Count()
                *
                map_lines.Where((map_line, index) => map_line[(index * 7) % line_length] == '#')
                         .Count()
                *
                map_lines.Where((map_line, index) => index % 2 == 0)
                         .Where((map_line, index) => map_line[(index * 1) % line_length] == '#')
                         .Count()
            );
        }
    }
}