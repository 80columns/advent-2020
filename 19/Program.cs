using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace _19 {
    class Program {
        static string ParseRules(Dictionary<string, object> _rules_dictionary, object _rule) {
            if (_rule.GetType() == typeof(ValueTuple<string, string>)) {
                var rule_items = (ValueTuple<string, string>)_rule;
                var first_part = string.Empty;
                var second_part = string.Empty;

                if (rule_items.Item1.Contains(' ')) {
                    foreach (var rule_part in rule_items.Item1.Split(' ')) {
                        first_part += ParseRules(_rules_dictionary, _rules_dictionary[rule_part]);
                    }
                } else {
                    first_part = ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item1]);
                }

                if (rule_items.Item2.Contains(' ')) {
                    foreach (var rule_part in rule_items.Item2.Split(' ')) {
                        second_part += ParseRules(_rules_dictionary, _rules_dictionary[rule_part]);
                    }
                } else {
                    second_part = ParseRules(_rules_dictionary, _rules_dictionary[rule_items.Item2]);
                }

                return "(" + first_part + "|" + second_part + ")";
            } else if (_rule.GetType() == typeof(string)) {
                if ((_rule as string).Contains(' ')) {
                    var rule_items = (_rule as string).Split(' ');
                    var output = "(";

                    for (var i = 0; i < rule_items.Length; i++) {
                        output = $"{output}{ParseRules(_rules_dictionary, _rules_dictionary[Convert.ToString(rule_items[i])])}";
                    }

                    return $"{output})";
                } else {
                    return "(" + ParseRules(_rules_dictionary, _rules_dictionary[(_rule as string)]) + ")";
                }
            } else {
                // if the rule is a single character, return it as a string
                return Convert.ToString(_rule);
            }
        }

        static Dictionary<string, object> GetRulesDictionary(string[] _rules) {
            var rules_dictionary = new Dictionary<string, object>();

            foreach (var rule in _rules) {
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

            return rules_dictionary;
        }

        static int GetMatchingMessageCount(string _rule, string[] _messages) {
            var matching_message_count = 0;

            foreach (var message in _messages) {
                var rule_index = 0;
                var message_index = 0;
                var message_index_stack = new Stack<int>();
                var previous_match = false;

                for (; rule_index < _rule.Length && message_index < message.Length; rule_index++) {
                    if (_rule[rule_index] == '(') {
                        // group open
                        message_index_stack.Push(message_index);
                    } else if (_rule[rule_index] == '|') {
                        // or character
                        if (previous_match) {
                            // don't need to check the right-hand side of the or statement, so find the closing parentheses index of the statement
                            var found_right_paren = false;
                            var existing_left_paren_count = 0;

                            while (!found_right_paren) {
                                rule_index++;

                                if (_rule[rule_index] == '(') {
                                    existing_left_paren_count++;
                                } else if (_rule[rule_index] == ')') {
                                    if (existing_left_paren_count > 0) {
                                        existing_left_paren_count--;
                                    } else {
                                        found_right_paren = true;
                                        message_index_stack.Pop();
                                    }
                                }
                            }
                        }
                    } else if (_rule[rule_index] == ')') {
                        // group close
                        message_index_stack.Pop();
                    } else {
                        // letter
                        if (_rule[rule_index] == message[message_index]) {
                            message_index++;
                            previous_match = true;
                        } else {
                            previous_match = false;

                            var reset_message_index = (_rule[rule_index - 1] == '(') ? false : true;
                            var found_pipe = false;
                            var existing_left_paren_count = 0;

                            while (!found_pipe) {
                                rule_index++;

                                if (rule_index == _rule.Length) {
                                    break;
                                } else if (_rule[rule_index] == '(') {
                                    existing_left_paren_count++;
                                } else if (_rule[rule_index] == ')' && existing_left_paren_count > 0) {
                                    existing_left_paren_count--;
                                } else if (_rule[rule_index] == ')' && existing_left_paren_count == 0) {
                                    message_index_stack.Pop();

                                    if (message_index_stack.Count > 0) {
                                        message_index = message_index_stack.Peek();
                                    }
                                } else if (_rule[rule_index] == '|' && existing_left_paren_count == 0) {
                                    found_pipe = true;

                                    if (reset_message_index) {
                                        message_index = message_index_stack.Peek();
                                    }
                                }
                            }
                        }
                    }
                }

                if (message_index == message.Length) {
                    matching_message_count++;
                }
            }

            return matching_message_count;
        }

        static async Task Main(string[] args) {
            var input1 = (await File.ReadAllTextAsync("input1.txt", Encoding.UTF8)).Split("\n\n");
            var rules1 = input1[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var messages1 = input1[1].Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var rules_dictionary = GetRulesDictionary(rules1);
            var rule = ParseRules(rules_dictionary, rules_dictionary["0"]);

            Console.WriteLine(GetMatchingMessageCount(rule, messages1));

            var input2 = (await File.ReadAllTextAsync("input2.txt", Encoding.UTF8)).Split("\n\n");
            var rules2 = input1[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var messages2 = input1[1].Split('\n', StringSplitOptions.RemoveEmptyEntries);

            

            Console.WriteLine("Hello World!");
        }
    }
}