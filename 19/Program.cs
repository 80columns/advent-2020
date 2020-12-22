﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace _19 {
    class Program {
        static string ParseRules(Dictionary<string, object> _rules_dictionary, object rule) {
            if (rule.GetType() == typeof(ValueTuple<string, string>)) {
                var rule_items = (ValueTuple<string, string>)rule;
                var first_part = string.Empty;
                var second_part = string.Empty;

                if (rule_items.Item1.Contains(' ')) {
                    first_part = ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item1.Split(' ')[0]])
                               + ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item1.Split(' ')[1]]);
                } else {
                    first_part = ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item1]);
                }

                if (rule_items.Item2.Contains(' ')) {
                    second_part = ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item2.Split(' ')[0]])
                                + ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item2.Split(' ')[1]]);
                } else {
                    second_part = ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item2]);
                }

                return "(" + first_part + "|" + second_part + ")";
            } else if (rule.GetType() == typeof(string)) {
                if ((rule as string).Contains(' ')) {
                    var rule_items = (rule as string).Split(' ');
                    var output = "(";

                    for (var i = 0; i < rule_items.Length; i++) {
                        output = $"{output}{ParseRules(_rules_dictionary, _rules_dictionary[Convert.ToString(rule_items[i])])}";
                    }

                    return $"{output})";
                } else {
                    return "(" + ParseRules(_rules_dictionary, _rules_dictionary[(rule as string)]) + ")";
                }
            } else {
                return Convert.ToString(rule);
            }
        }

        static async Task Main(string[] args) {
            var input = (await File.ReadAllTextAsync("input.txt", Encoding.UTF8)).Split("\n\n");
            
            var rules = input[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var messages = input[1].Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var rules_dictionary = new Dictionary<string, object>();

            foreach (var rule in rules) {
                var rule_parts = rule.Split(':');
                var rule_conditions = rule_parts[1];
                var rule_value = null as object;

                if (rule_conditions.Contains("|")) {
                    var first_condition = rule_conditions.Split('|')[0].Trim();
                    var second_condition = rule_conditions.Split('|')[1].Trim();

                    rule_value = (first_condition, second_condition);
                } else if (rule_conditions.Contains("\"")) {
                    rule_value = rule_conditions.Trim().Replace("\"", string.Empty).ToCharArray()[0];
                } else {
                    rule_value = rule_conditions.Trim();
                }

                rules_dictionary.Add(rule_parts[0], rule_value);
            }

            var parsed_rule = ParseRules(rules_dictionary, rules_dictionary["0"]);

            Console.WriteLine(parsed_rule);

            var matching_message_count = 0;

            foreach (var message in messages) {
                var message_index = 0;
                var message_index_stack = new Stack<int>();
                var previous_match = false;
                var previous_char_right_paren = false;

                Console.WriteLine($"processing message {message}...");
                Console.WriteLine();

                for (var rule_index = 0; rule_index < parsed_rule.Length && message_index < message.Length; rule_index++) {
                    Console.WriteLine($"ri => {rule_index}");
                    Console.WriteLine($"rule[ri] => {parsed_rule[rule_index]}");
                    Console.WriteLine($"mi => {message_index}");
                    Console.WriteLine($"message[mi] => {message[message_index]}");
                    Console.WriteLine($"previous_match => {previous_match}");
                    Console.WriteLine($"mi stack => {string.Join(",", message_index_stack)}");
                    Console.WriteLine();

                    if (parsed_rule[rule_index] == '(') {
                        // group open
                        if (!previous_char_right_paren) {
                            message_index_stack.Push(message_index);
                        }

                        previous_char_right_paren = false;
                    } else if (parsed_rule[rule_index] == '|') {
                        // or
                        if (previous_match) {
                            // don't need to check the right-hand side of the or statement, so find the closing parentheses index of the statement
                            var found_right_paren = false;
                            var existing_left_paren_count = 0;

                            while (!found_right_paren) {
                                rule_index++;

                                if (parsed_rule[rule_index] == '(') {
                                    existing_left_paren_count++;
                                } else if (parsed_rule[rule_index] == ')') {
                                    if (existing_left_paren_count > 0) {
                                        existing_left_paren_count--;
                                    } else {
                                        found_right_paren = true;
                                    }
                                }
                            }
                        }

                        previous_char_right_paren = false;
                    } else if (parsed_rule[rule_index] == ')') {
                        // group close
                        previous_char_right_paren = true;
                    } else {
                        // letter
                        if (parsed_rule[rule_index] == message[message_index]) {
                            message_index++;
                            previous_match = true;
                        } else {
                            previous_match = false;

                            var found_pipe = false;
                            var existing_left_paren_count = 0;

                            while (!found_pipe) {
                                rule_index++;

                                if (parsed_rule[rule_index] == '(') {
                                    existing_left_paren_count++;
                                } else if (parsed_rule[rule_index] == ')' && existing_left_paren_count > 0) {
                                    existing_left_paren_count--;
                                } else if (parsed_rule[rule_index] == ')' && existing_left_paren_count == 0) {
                                    message_index = message_index_stack.Pop();
                                } else if (parsed_rule[rule_index] == '|' && existing_left_paren_count == 0) {
                                    found_pipe = true;
                                }
                            }
                        }
                    
                        previous_char_right_paren = false;
                    }
                }

                if (message_index == message.Length) {
                    Console.WriteLine($"message {message} matched!");
                    Console.WriteLine();
                    matching_message_count++;
                }
            }

            Console.WriteLine(matching_message_count);

            Console.WriteLine("Hello World!");
        }
    }
}