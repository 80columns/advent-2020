using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _23 {
    class CircularListNode {
        public long value { get; private set; }
        public CircularListNode Next;

        public CircularListNode(long _value) {
            this.value = _value;
            this.Next = null;
        }

        public CircularListNode FindNode(long _value) {
            var next_node = this.Next;

            // normally we'd check to ensure that we haven't looped back around to this current node,
            // but since we know this wouldn't happen for this problem this check isn't included here
            while (next_node.value != _value) {
                next_node = next_node.Next;
            }

            return next_node;
        }
    }

    class Program {
        static void Main(string[] args) {
            // part 1
            var part_1_cups = "362981754".Select(c => Convert.ToInt64(c.ToString())).ToList();
            
            move_cups(ref part_1_cups, 100);

            var cups_after_one = new StringBuilder();
            var output_cup_index = part_1_cups.IndexOf(1) + 1;

            for (var i = 0; i < part_1_cups.Count - 1; i++, output_cup_index++) {
                if (output_cup_index == part_1_cups.Count) {
                    output_cup_index = 0;
                }

                cups_after_one.Append(part_1_cups[output_cup_index].ToString());
            }

            Console.WriteLine(cups_after_one.ToString());

            // part 2
            var part_2_cups = "362981754".Select(c => Convert.ToInt64(c.ToString())).ToList();
            var part_2_cup_dictionary = new Dictionary<long, CircularListNode>();
            var part_2_cups_max = part_2_cups.Max();
            var part_2_cups_min = part_2_cups.Min();

            for (var i = part_2_cups_max + 1; i <= 1_000_000; i++) {
                part_2_cups.Add(i);
            }

            part_2_cups_max = 1_000_000;

            var list_head = new CircularListNode(part_2_cups[0]);
            var current_cup = list_head;
            part_2_cup_dictionary[part_2_cups[0]] = list_head;

            for (var i = 1; i < part_2_cups.Count; i++) {
                current_cup.Next = new CircularListNode(part_2_cups[i]);
                current_cup = current_cup.Next;
                part_2_cup_dictionary[part_2_cups[i]] = current_cup;
            }

            current_cup.Next = list_head;
            
            move_cups(ref list_head, 10_000_000, part_2_cups_min, part_2_cups_max, part_2_cup_dictionary);
            
            Console.WriteLine(part_2_cup_dictionary[1].Next.value * part_2_cup_dictionary[1].Next.Next.value);
        }

        // the slow way
        static void move_cups(ref List<long> _cups, int _moves) {
            var current_cup = _cups[0];
            var highest_cup = _cups.Max();

            // perform moves
            for (var i = 0; i < _moves; i++) {
                var end_cup_count = 3;
                var beginning_cup_count = 0;
                var current_cup_index = _cups.IndexOf(current_cup);

                if (_cups.Count - (current_cup_index + 1) < 3) {
                    end_cup_count = _cups.Count - (current_cup_index + 1);
                    beginning_cup_count = 3 - end_cup_count;
                }

                var three_cups = _cups.GetRange(current_cup_index + 1, end_cup_count);

                if (beginning_cup_count > 0) {
                    three_cups.AddRange(_cups.GetRange(0, beginning_cup_count));
                }

                _cups.RemoveAll(cup => three_cups.Contains(cup));

                var destination_cup = current_cup - 1;

                while (three_cups.Contains(destination_cup)) {
                    destination_cup--;
                }

                if (_cups.Contains(destination_cup) == false) {
                    destination_cup = highest_cup;
                }

                while (three_cups.Contains(destination_cup)) {
                    destination_cup--;
                }

                var destination_cup_index = _cups.IndexOf(destination_cup);

                _cups.InsertRange(destination_cup_index + 1, three_cups);

                current_cup_index = _cups.IndexOf(current_cup);
                current_cup = (current_cup_index + 1 == _cups.Count) ? _cups[0] : _cups[current_cup_index + 1];
            }
        }

        // the fast way
        static void move_cups(ref CircularListNode _current_cup, int _moves, long _min_value, long _max_value, Dictionary<long, CircularListNode> _cup_dictionary) {
            // perform moves
            for (var i = 0; i < _moves; i++) {
                var triplet_head = _current_cup.Next;

                _current_cup.Next = triplet_head.Next.Next.Next;
                triplet_head.Next.Next.Next = null;

                var destination_cup_value = _current_cup.value - 1;

                while (
                    triplet_head.value == destination_cup_value
                 || triplet_head.Next.value == destination_cup_value
                 || triplet_head.Next.Next.value == destination_cup_value
                ) {
                    destination_cup_value--;
                }

                if (destination_cup_value < _min_value) {
                    destination_cup_value = _max_value;
                }

                while (
                    triplet_head.value == destination_cup_value
                 || triplet_head.Next.value == destination_cup_value
                 || triplet_head.Next.Next.value == destination_cup_value
                ) {
                    destination_cup_value--;
                }

                var destination_cup_node = _cup_dictionary[destination_cup_value];

                triplet_head.Next.Next.Next = destination_cup_node.Next;
                destination_cup_node.Next = triplet_head;

                _current_cup = _current_cup.Next;
            }
        }
    }
}