using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _4 {
    class Program {
        static async Task Main(string[] args) {
            var passports = await File.ReadAllTextAsync("input.txt", System.Text.Encoding.UTF8);

            /*
             * Part 1
             *
             * Valid fields only, no value validation
             *
             */
            Console.WriteLine(
                passports.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(passport => passport.Replace('\n', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .Select(passport => passport.ToDictionary(field => field.Split(':')[0], field => field.Split(':')[1]))
                    .Where(fields => (new HashSet<string>() { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" }).All(valid_field => fields.ContainsKey(valid_field)))
                    .Count()
            );

            /*
             * Part 2
             *
             * Valid fields with value validation
             *
             */
            Console.WriteLine(
                passports.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
                    .Select(passport => passport.Replace('\n', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .Select(passport => passport.ToDictionary(field => field.Split(':')[0], field => field.Split(':')[1]))
                    .Where(fields => (new HashSet<string>() { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" }).All(valid_field => fields.ContainsKey(valid_field)))
                    .Where(fields => short.TryParse(fields["byr"], out short byr) && byr >= 1920 && byr <= 2002)
                    .Where(fields => short.TryParse(fields["iyr"], out short iyr) && iyr >= 2010 && iyr <= 2020)
                    .Where(fields => short.TryParse(fields["eyr"], out short eyr) && eyr >= 2020 && eyr <= 2030)
                    .Where(
                        fields => (fields["hgt"].EndsWith("cm") && byte.TryParse(fields["hgt"].Substring(0, fields["hgt"].Length - 2), out byte hgt_cm) && hgt_cm >= 150 && hgt_cm <= 193)
                               || (fields["hgt"].EndsWith("in") && byte.TryParse(fields["hgt"].Substring(0, fields["hgt"].Length - 2), out byte hgt_in) && hgt_in >= 59 && hgt_in <= 76)
                    )
                    .Where(fields => fields["hcl"].StartsWith('#') && Regex.IsMatch(fields["hcl"].Substring(1, fields["hcl"].Length - 1), "^[0-9a-f]{6}$"))
                    .Where(fields => (new HashSet<string>() { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }).Contains(fields["ecl"]))
                    .Where(fields => Regex.IsMatch(fields["pid"], "^[0-9]{9}$"))
                    .Count()
            );
        }
    }
}