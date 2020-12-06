using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5 {
    static class Program {
        static IEnumerable<(T Previous, T Current)> GetPreviousAndCurrent<T>(this IEnumerable<T> source) {
            T previous = default(T);

            foreach (var current in source) {
                yield return (previous, current);

                previous = current;
            }
        }

        static async Task Main(string[] args) {
            var seat_assignments = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Find highest seat ID
             */

            // sa = seat assignment
            // ri = row index
            // rli = row lower index
            // rui = row upper index
            // ci = column index
            // cli = column lower index
            // cui = column upper index
            // cd = code
            Console.WriteLine(
                seat_assignments.Select(sa => (rli: 0, rui: 127, cli: 0, cui: 7, cd: sa))
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (ri: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (ri: sa.rli, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'R' ?
                                        (ri: sa.ri, cli: sa.cui - ((sa.cui - sa.cli) / 2), cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (ri: sa.ri, cli: sa.cli, cui: sa.cli + ((sa.cui - sa.cli) / 2), cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'R' ?
                                        (ri: sa.ri, cli: sa.cui - ((sa.cui - sa.cli) / 2), cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (ri: sa.ri, cli: sa.cli, cui: sa.cli + ((sa.cui - sa.cli) / 2), cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'R' ?
                                        (ri: sa.ri, ci: sa.cui)
                                      : (ri: sa.ri, ci: sa.cli)
                                )
                                .Select(sa => (sa.ri * 8) + sa.ci)
                                .Max()
            );

            /*
             * Part 2
             *
             * Find your seat ID
             */
            Console.WriteLine(
                seat_assignments.Select(sa => (rli: 0, rui: 127, cli: 0, cui: 7, cd: sa))
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (rli: sa.rui - ((sa.rui - sa.rli) / 2), rui: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (rli: sa.rli, rui: sa.rli + ((sa.rui - sa.rli) / 2), cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'B' ?
                                        (ri: sa.rui, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (ri: sa.rli, cli: sa.cli, cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'R' ?
                                        (ri: sa.ri, cli: sa.cui - ((sa.cui - sa.cli) / 2), cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (ri: sa.ri, cli: sa.cli, cui: sa.cli + ((sa.cui - sa.cli) / 2), cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'R' ?
                                        (ri: sa.ri, cli: sa.cui - ((sa.cui - sa.cli) / 2), cui: sa.cui, cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                      : (ri: sa.ri, cli: sa.cli, cui: sa.cli + ((sa.cui - sa.cli) / 2), cd: sa.cd.Substring(1, sa.cd.Length - 1))
                                )
                                .Select(
                                    sa => sa.cd[0] == 'R' ?
                                        (ri: sa.ri, ci: sa.cui)
                                      : (ri: sa.ri, ci: sa.cli)
                                )
                                .Select(sa => (sa.ri * 8) + sa.ci)
                                .OrderBy(sa => sa)
                                .GetPreviousAndCurrent()
                                .Where(pc => pc.Current - pc.Previous == 2)
                                .Select(pc => pc.Current - 1)
                                .First()
            );
        }
    }
}