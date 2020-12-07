using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6 {
    class Program {
        static async Task Main(string[] args) {
            var customs_declaration_forms = (await File.ReadAllTextAsync("input.txt", Encoding.UTF8)).Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

            /*
             * Part 1
             *
             * Find sum of questions to which anyone answered "yes"
             */
            Console.WriteLine(
                customs_declaration_forms.Select(
                                              form => form.Replace("\n", string.Empty)
                                                          .Distinct()
                                                          .Count()
                                         )
                                         .Sum()
            );

            /*
             * Part 2
             *
             * Find sum of questions to which everyone answered "yes"
             */
            Console.WriteLine(
                customs_declaration_forms.Select(form => (group_count: form.TrimEnd('\n').Count(character => character == '\n') + 1, responses: form.Replace("\n", string.Empty)))
                                         .Select(form => form.responses.Distinct().Count(distinct_answer => form.responses.Count(answer => answer == distinct_answer) == form.group_count))
                                         .Sum()
            );
        }
    }
}