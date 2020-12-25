using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace _19 {
    class Program {
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

        static bool MatchMessage(string _rule, string _message) {
            var rule_index = 0;
            var message_index = 0;
            var message_index_stack = new Stack<int>();
            var previous_match = false;

            for (; rule_index < _rule.Length && message_index < _message.Length; rule_index++) {
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
                    if (_rule[rule_index] == _message[message_index]) {
                        message_index++;
                        previous_match = true;

                        if (message_index == _message.Length) {
                            var left_paren_count = 0;

                            // validate we're at a valid ending for the rule - traverse until at the first right paren corresponding to the last left paren where 0 was pushed on the stack
                            while (message_index_stack.Count > 0) {
                                rule_index++;

                                if (rule_index == _rule.Length) {
                                    break;
                                } else if (_rule[rule_index] == '(') {
                                    left_paren_count++;
                                } else if (_rule[rule_index] == ')' && left_paren_count > 0) {
                                    left_paren_count--;
                                } else if (_rule[rule_index] == ')' && left_paren_count == 0) {
                                    message_index_stack.Pop();

                                    if (rule_index < _rule.Length - 1 && _rule[rule_index + 1] != ')' && _rule[rule_index + 1] != '|') {
                                        previous_match = false;

                                        var found_pipe = false;
                                        var inner_left_paren_count = 0;

                                        while (!found_pipe) {
                                            rule_index++;

                                            if (rule_index == _rule.Length) {
                                                break;
                                            } else if (_rule[rule_index] == '(') {
                                                inner_left_paren_count++;
                                            } else if (_rule[rule_index] == ')' && inner_left_paren_count > 0) {
                                                inner_left_paren_count--;
                                            } else if (_rule[rule_index] == ')' && inner_left_paren_count == 0) {
                                                message_index_stack.Pop();

                                                if (message_index_stack.Count > 0) {
                                                    message_index = message_index_stack.Peek();
                                                }
                                            } else if (_rule[rule_index] == '|' && inner_left_paren_count == 0) {
                                                found_pipe = true;

                                                if (message_index_stack.Count > 0) {
                                                    message_index = message_index_stack.Peek();
                                                } else {
                                                    message_index = 0;
                                                }
                                            }
                                        }

                                        // the while loop above exited when rule_index = _rule.Length
                                        // as no pipe was found 
                                        if (!found_pipe) {
                                            message_index = 0;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    } else {
                        previous_match = false;

                        var reset_message_index = (_rule[rule_index - 1] == '(') ? false : true;
                        var found_pipe = false;
                        var left_paren_count = 0;

                        while (!found_pipe) {
                            rule_index++;

                            if (rule_index == _rule.Length) {
                                break;
                            } else if (_rule[rule_index] == '(') {
                                left_paren_count++;
                            } else if (_rule[rule_index] == ')' && left_paren_count > 0) {
                                left_paren_count--;
                            } else if (_rule[rule_index] == ')' && left_paren_count == 0) {
                                message_index_stack.Pop();

                                if (message_index_stack.Count > 0) {
                                    message_index = message_index_stack.Peek();
                                }
                            } else if (_rule[rule_index] == '|' && left_paren_count == 0) {
                                found_pipe = true;

                                if (reset_message_index) {
                                    message_index = message_index_stack.Peek();
                                }
                            }
                        }
                    }
                }
            }

            return message_index == _message.Length;
        }

        static HashSet<string> GetMatchingMessages(string _rule, HashSet<string> _messages) {
            var matched_messages = new HashSet<string>();

            foreach (var message in _messages) {
                if (MatchMessage(_rule, message)) {
                    matched_messages.Add(message);
                }
            }

            return matched_messages;
        }

        static HashSet<string> GetNonBeginEndMatchingMessages(string _rule42s, string _rule31s, HashSet<string> _unmatched_messages) {
            var non_beginning_ending_matches = new HashSet<string>();

            // get unmatched messages that don't have (1) a beginning substring that matches (1 or more) rule 42 and (2) an ending substring that matches (1 or more) rule 31
            foreach (var unmatched_message in _unmatched_messages) {
                var matched_rule_42s = false;
                var matched_rule_31s = false;
                var beginning_substring_length = 1;
                var ending_substring_begin_index = unmatched_message.Length - 1;

                for (; beginning_substring_length < unmatched_message.Length; beginning_substring_length++) {
                    if (MatchMessage(_rule42s, unmatched_message.Substring(0, beginning_substring_length))) {
                        matched_rule_42s = true;
                        break;
                    }
                }

                for (; ending_substring_begin_index >= 1; ending_substring_begin_index--) {
                    if (MatchMessage(_rule31s, unmatched_message.Substring(ending_substring_begin_index))) {
                        matched_rule_31s = true;
                        break;
                    }
                }

                if (!matched_rule_42s || !matched_rule_31s || beginning_substring_length > ending_substring_begin_index) {
                    non_beginning_ending_matches.Add(unmatched_message);
                }
            }

            return non_beginning_ending_matches;
        }

        static HashSet<string> GetNonBeginMatchingMessages(string _rule42s, HashSet<string> _unmatched_messages) {
            var non_beginning_matches = new HashSet<string>();

            // get unmatched messages that don't have (1) a beginning substring that matches (1 or more) rule 42
            foreach (var unmatched_message in _unmatched_messages) {
                var matched_rule_42s = false;
                var beginning_substring_length = 1;

                for (; beginning_substring_length < unmatched_message.Length; beginning_substring_length++) {
                    if (MatchMessage(_rule42s, unmatched_message.Substring(0, beginning_substring_length))) {
                        matched_rule_42s = true;
                        break;
                    }
                }

                if (!matched_rule_42s) {
                    non_beginning_matches.Add(unmatched_message);
                }
            }

            return non_beginning_matches;
        }

        static async Task Main(string[] args) {
            var input = (await File.ReadAllTextAsync("input.txt", Encoding.UTF8)).Split("\n\n");
            var rules = input[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var messages = new HashSet<string>(input[1].Split('\n', StringSplitOptions.RemoveEmptyEntries));

            /*
             * Part 1
             *
             * How many messages completely match rule 0?
             */
            var rules_dictionary = GetRulesDictionary(rules);
            var rule_0 = ParseRules(rules_dictionary, rules_dictionary["0"]);
            var matching_messages = GetMatchingMessages(rule_0, messages);

            Console.WriteLine(matching_messages.Count);

            /*
             * Part 2
             *
             * How many messages completely match rule 0 with the modifications below?
             */
            var rule_42 = ParseRules(rules_dictionary, rules_dictionary["42"]);
            var rule_31 = ParseRules(rules_dictionary, rules_dictionary["31"]);

            // 0: 8 11
            // 8: 42 => 8: 42 | 42 8
            // 11: 42 31 => 11: 42 31 | 42 11 31
            //
            // 8 (0 substitution): (42 | 42 8)
            // 8 (1 substitution): (42 | 42 (42 | 42 8))
            // 8 (2 substitution): (42 | 42 (42 | 42 (42 | 42 8)))
            //
            // 11 (0 substitution): (42 31 | 42 11 31)
            // 11 (1 substitution): (42 31 | 42 (42 31 | 42 11 31) 31)
            // 11 (2 substitution): (42 31 | 42 (42 31 | 42 (42 31 | 42 11 31) 31) 31)
            //
            // (42 | 42 8)(42 31 | 42 11 31)
            // 
            // 8: (42+)
            // 11: (42){n}(31){n} (n >= 1)
            //
            // at least one 42 rule, then n (>=1) or more 42 rules followed by n (>=1) or more 31 rules
            var rule_8_permutations = new HashSet<string>() { $"({rule_42})" };
            var rule_11_permutations = new HashSet<string>() { $"({rule_42})({rule_31})" };
            var tested_rule_permutations = new HashSet<string>();

            var unmatched_messages = messages;
            unmatched_messages.ExceptWith(matching_messages);

            // remove any messages that don't begin with rule 42 or don't end with rule 31 in a non-overlapping manner
            unmatched_messages.ExceptWith(GetNonBeginEndMatchingMessages(rule_42, rule_31, unmatched_messages));

            // skip repeating_rule_count = 1 because this is solved in part 1
            var repeating_rule_count = 2;

            while (unmatched_messages.Count > 0) {
                var generated_rule_8 = string.Empty;
                var generated_rule_11 = string.Empty;
                var common_rule_42_prefix = string.Empty;
                var new_rule_permutations = new HashSet<string>();

                // add the next recursion of rule 8 + 11 to their respective lists
                // rule 8:
                // (42 (42))
                // (42 (42 (42)))
                // (42 (42 (42 (42))))
                // ...
                //
                // rule 11:
                // (42 (42 31) 31)
                // (42 (42 (42 31) 31) 31)
                // (42 (42 (42 (42 31) 31) 31) 31)
                // ...

                // rule 8
                for (var i = 0; i < repeating_rule_count; i++) {
                    generated_rule_8 += $"({rule_42}";
                }

                for (var i = 0; i < repeating_rule_count; i++) {
                    generated_rule_8 += ")";
                }

                // rule 11 (1st half)
                for (var i = 0; i < repeating_rule_count; i++) {
                    generated_rule_11 += $"({rule_42}";
                }

                // rule 11 (2nd half)
                for (var i = 0; i < repeating_rule_count; i++) {
                    generated_rule_11 += $"{rule_31})";
                }

                rule_8_permutations.Add(generated_rule_8);
                rule_11_permutations.Add(generated_rule_11);

                // generate the total set of rules generated by the cross product
                // of the rule 8s and rule 11s, only keeping those that haven't been evaluated previously
                foreach (var rule_8 in rule_8_permutations) {
                    foreach (var rule_11 in rule_11_permutations) {
                        if (!tested_rule_permutations.Contains($"{rule_8}{rule_11}")) {
                            new_rule_permutations.Add($"{rule_8}{rule_11}");
                        }
                    }
                }

                // preemptively remove any unmatched messages that don't have a beginning substring that matches
                // rule 42 (repeating_rule_count + 1) times
                // this is the minimum number of times rule 42 could appear at the beginning of any of the new rules,
                // because this count is defined by the current permutation of rule 11 appended to the 1st permutation of rule 8 (i.e. the instance with 1 rule 42)
                for (var i = 0; i < repeating_rule_count + 1; i++) {
                    common_rule_42_prefix += rule_42;
                }

                unmatched_messages.ExceptWith(GetNonBeginMatchingMessages(common_rule_42_prefix, unmatched_messages));

                // match the remaining messages against the new set of rules generated by the cross product
                // of the rule 8s and rule 11s
                foreach (var new_rule in new_rule_permutations) {
                    var temp_matching_messages = GetMatchingMessages(new_rule, unmatched_messages);

                    matching_messages.UnionWith(temp_matching_messages);
                    unmatched_messages.ExceptWith(temp_matching_messages);

                    tested_rule_permutations.Add(new_rule);
                }

                // increment the rule recursion for next iteration
                repeating_rule_count++;
            }

            Console.WriteLine(matching_messages.Count);
        }
    }
}