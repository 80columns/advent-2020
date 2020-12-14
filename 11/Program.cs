using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11 {
    class Program {
        static async Task Main(string[] args) {
            /*
             * Part 1
             *
             * Predict eventual seat placement based on these rules:
             * - If a seat is empty (L) and there are no occupied seats adjacent to it, the seat becomes occupied.
             * - If a seat is occupied (#) and four or more seats adjacent to it are also occupied, the seat becomes empty.
             * - Otherwise, the seat's state does not change.
             */
            var seat_map = (await File.ReadAllLinesAsync("input.txt", Encoding.UTF8));
            var seats = new Dictionary<(int i, int j), (char val, char new_val)>();

            var i_len = seat_map.Count();
            var j_len = seat_map[0].Length;

            for (var i = 0; i < i_len; i++) {
                for (var j = 0; j < j_len; j++) {
                    seats[(i, j)] = (seat_map[i][j], ' ');
                }
            }

            var seats_changed = true;

            while (seats_changed) {
                seats_changed = false;

                for (var i = 0; i < i_len; i++) {
                    var top = i-1;
                    var bottom = i+1;
                    var upper_i = i_len-1;
                    var lower_i = 0;

                    for (var j = 0; j < j_len; j++) {
                        var left = j-1;
                        var right = j+1;
                        var upper_j = j_len-1;
                        var lower_j = 0;

                        var left_occupied = j > lower_j && seats[(i, left)].val == '#';
                        var top_left_occupied = i > lower_i && j > lower_j && seats[(top, left)].val == '#';
                        var top_occupied = i > lower_i && seats[(top, j)].val == '#';
                        var top_right_occupied = i > lower_i && j < upper_j && seats[(top, right)].val == '#';
                        var right_occupied = j < upper_j && seats[(i, right)].val == '#';
                        var bottom_right_occupied = i < upper_i && j < upper_j && seats[(bottom, right)].val == '#';
                        var bottom_occupied = i < upper_i && seats[(bottom, j)].val == '#';
                        var bottom_left_occupied = i < upper_i && j > lower_j && seats[(bottom, left)].val == '#';

                        if (seats[(i, j)].val == 'L') {
                            if (
                                (!left_occupied || j == lower_j)
                             && (!top_left_occupied || i == lower_i || j == lower_j)
                             && (!top_occupied || i == lower_i)
                             && (!top_right_occupied || i == lower_i || j == upper_j)
                             && (!right_occupied || j == upper_j)
                             && (!bottom_right_occupied || i == upper_i || j == upper_j)
                             && (!bottom_occupied || i == upper_i)
                             && (!bottom_left_occupied || i == upper_i || j == lower_j)
                            ) {
                                seats[(i, j)] = (seats[(i, j)].val, '#');
                                seats_changed = true;
                            }
                        } else if (seats[(i, j)].val == '#') {
                            if (
                                Convert.ToInt32(left_occupied) +
                                Convert.ToInt32(top_left_occupied) +
                                Convert.ToInt32(top_occupied) +
                                Convert.ToInt32(top_right_occupied) +
                                Convert.ToInt32(right_occupied) +
                                Convert.ToInt32(bottom_right_occupied) +
                                Convert.ToInt32(bottom_occupied) +
                                Convert.ToInt32(bottom_left_occupied)
                                >= 4
                            ) {
                                seats[(i, j)] = (seats[(i, j)].val, 'L');
                                seats_changed = true;
                            }
                        }
                    }
                }

                for (var i = 0; i < i_len; i++) {
                    for (var j = 0; j < j_len; j++) {
                        if (seats[(i, j)].new_val != ' ') {
                            seats[(i, j)] = (seats[(i, j)].new_val, ' ');
                        }
                    }
                }
            }

            Console.WriteLine(seats.Count(x => x.Value.val == '#'));

            /*
             * Part 2
             *
             * Predict eventual seat placement based on these rules:
             * - If a seat is empty (L) and there are no occupied seats in any of the 8 directions from it, the seat becomes occupied.
             * - If a seat is occupied (#) and five or more seats in any of the 8 directions from it are also occupied, the seat becomes empty.
             * - Otherwise, the seat's state does not change.
             */
            for (var i = 0; i < i_len; i++) {
                for (var j = 0; j < j_len; j++) {
                    seats[(i, j)] = (seat_map[i][j], ' ');
                }
            }

            seats_changed = true;

            while (seats_changed) {
                seats_changed = false;

                for (var i = 0; i < i_len; i++) {
                    var upper_i = i_len-1;
                    var lower_i = 0;

                    for (var j = 0; j < j_len; j++) {
                        var upper_j = j_len-1;
                        var lower_j = 0;

                        var first_left_seat_occupied = seats.Where(z => z.Key.i == i && z.Key.j < j && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.j).LastOrDefault().Value.val == '#';
                        var first_top_left_seat_occupied = seats.Where(z => z.Key.i < i && z.Key.j < j && (i - z.Key.i) == (j - z.Key.j) && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.j).LastOrDefault().Value.val == '#';
                        var first_top_seat_occupied = seats.Where(z => z.Key.i < i && z.Key.j == j && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.i).LastOrDefault().Value.val == '#';
                        var first_top_right_seat_occupied = seats.Where(z => z.Key.i < i && z.Key.j > j && (i - z.Key.i) == (z.Key.j - j) && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.i).LastOrDefault().Value.val == '#';

                        var first_right_seat_occupied = seats.Where(z => z.Key.i == i && z.Key.j > j && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.j).FirstOrDefault().Value.val == '#';
                        var first_bottom_right_seat_occupied = seats.Where(z => z.Key.i > i && z.Key.j > j && (z.Key.i - i) == (z.Key.j - j) && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.i).FirstOrDefault().Value.val == '#';
                        var first_bottom_seat_occupied = seats.Where(z => z.Key.i > i && z.Key.j == j && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.i).FirstOrDefault().Value.val == '#';
                        var first_bottom_left_seat_occupied = seats.Where(z => z.Key.i > i && z.Key.j < j && (z.Key.i - i) == (j - z.Key.j) && (z.Value.val == '#' || z.Value.val == 'L')).OrderBy(z => z.Key.i).FirstOrDefault().Value.val == '#';

                        if (seats[(i, j)].val == 'L') {
                            if (
                                (!first_left_seat_occupied || j == lower_j)
                             && (!first_top_left_seat_occupied || i == lower_i || j == lower_j)
                             && (!first_top_seat_occupied || i == lower_i)
                             && (!first_top_right_seat_occupied || i == lower_i || j == upper_j)
                             && (!first_right_seat_occupied || j == upper_j)
                             && (!first_bottom_right_seat_occupied || i == upper_i || j == upper_j)
                             && (!first_bottom_seat_occupied || i == upper_i)
                             && (!first_bottom_left_seat_occupied || i == upper_i || j == lower_j)
                            ) {
                                seats[(i, j)] = (seats[(i, j)].val, '#');
                                seats_changed = true;
                            }
                        } else if (seats[(i, j)].val == '#') {
                            if (
                                Convert.ToInt32(first_left_seat_occupied) +
                                Convert.ToInt32(first_top_left_seat_occupied) +
                                Convert.ToInt32(first_top_seat_occupied) +
                                Convert.ToInt32(first_top_right_seat_occupied) +
                                Convert.ToInt32(first_right_seat_occupied) +
                                Convert.ToInt32(first_bottom_right_seat_occupied) +
                                Convert.ToInt32(first_bottom_seat_occupied) +
                                Convert.ToInt32(first_bottom_left_seat_occupied)
                                >= 5
                            ) {
                                seats[(i, j)] = (seats[(i, j)].val, 'L');
                                seats_changed = true;
                            }
                        }
                    }
                }

                for (var i = 0; i < i_len; i++) {
                    for (var j = 0; j < j_len; j++) {
                        if (seats[(i, j)].new_val != ' ') {
                            seats[(i, j)] = (seats[(i, j)].new_val, ' ');
                        }
                    }
                }
            }

            Console.WriteLine(seats.Count(x => x.Value.val == '#'));
        }
    }
}