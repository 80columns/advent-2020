using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _18 {
    class Program {
        static async Task Main(string[] args) {
            var expressions = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Evaluate expressions without operator precedence
             */
            Console.WriteLine(expressions.Select(expression => EvaluateExpression(expression)).Sum());

            /*
             * Part 2
             *
             * Evaluate expressions with operator precedence (+ before *)
             */
            Console.WriteLine(expressions.Select(expression => {
                while (expression.Contains("+") || expression.Contains("*")) {
                    if (expression.Contains("(")) {
                        var left_parentheses_index = expression.IndexOf('(');
                        var right_parentheses_index = -1;

                        for (var j = left_parentheses_index + 1; j < expression.Length; j++) {
                            if (expression[j] == ')') {
                                right_parentheses_index = j;
                                break;
                            } else if (expression[j] == '(') {
                                // update the left parentheses index so that the innermost one is selected
                                left_parentheses_index = j;
                            }
                        }

                        var inner_expression = expression.Substring(left_parentheses_index, right_parentheses_index - left_parentheses_index + 1);

                        if (inner_expression.Contains("+") || inner_expression.Contains("*")) {
                            var op_index = (inner_expression.IndexOf('+') == -1) ? inner_expression.IndexOf('*') : inner_expression.IndexOf('+');

                            var first_number_start_index = (inner_expression.LastIndexOf(' ', op_index - 2) == -1) ? inner_expression.LastIndexOf('(', op_index - 2) + 1 : inner_expression.LastIndexOf(' ', op_index - 2) + 1;
                            var second_number_end_index = (inner_expression.IndexOf(' ', op_index + 2) == -1) ? inner_expression.IndexOf(')', op_index + 2) - 1 : inner_expression.IndexOf(' ', op_index + 2) - 1;

                            var first_number = Convert.ToInt64(inner_expression.Substring(first_number_start_index, op_index - first_number_start_index - 1));
                            var second_number = Convert.ToInt64(inner_expression.Substring(op_index + 2, second_number_end_index - op_index - 1));

                            var result = (inner_expression[op_index] == '+') ? first_number + second_number : first_number * second_number;

                            inner_expression = $"{inner_expression.Substring(0, first_number_start_index)}{result}{inner_expression.Substring(second_number_end_index + 1)}";
                            expression = $"{expression.Substring(0, left_parentheses_index)}{inner_expression}{expression.Substring(right_parentheses_index + 1)}";
                        } else {
                            expression = $"{expression.Substring(0, left_parentheses_index)}{inner_expression.Substring(1, inner_expression.Length - 2)}{expression.Substring(right_parentheses_index + 1)}";
                        }
                    } else {
                        var op_index = (expression.IndexOf('+') == -1) ? expression.IndexOf('*') : expression.IndexOf('+');

                        var first_number_start_index = (expression.LastIndexOf(' ', op_index - 2) == -1) ? 0 : expression.LastIndexOf(' ', op_index - 2) + 1;
                        var second_number_end_index = (expression.IndexOf(' ', op_index + 2) == -1) ? expression.Length - 1 : expression.IndexOf(' ', op_index + 2) - 1;

                        var first_number = Convert.ToInt64(expression.Substring(first_number_start_index, op_index - first_number_start_index - 1));
                        var second_number = Convert.ToInt64(expression.Substring(op_index + 2, second_number_end_index - op_index - 1));

                        var result = (expression[op_index] == '+') ? first_number + second_number : first_number * second_number;

                        expression = $"{expression.Substring(0, first_number_start_index)}{result}{expression.Substring(second_number_end_index + 1)}";
                    }
                }

                return Convert.ToInt64(expression);
            }).Sum());
        }

        static long EvaluateExpression(string _expression) {
            if (_expression.EndsWith(")")) {
                var right_parentheses_count = 0;
                var index = _expression.Length - 2;

                for (; index >= 0; index--) {
                    if (_expression[index] == ')') {
                        right_parentheses_count++;
                    } else if (_expression[index] == '(') {
                        if (right_parentheses_count == 0) {
                            break;
                        } else {
                            right_parentheses_count--;
                        }
                    }
                }

                // if this is just the first part of the expression, return its evaluation
                if (index == 0) {
                    return EvaluateExpression(_expression.Substring(1, _expression.Length - 2));
                } else {
                    var right_expression = _expression.Substring(index + 1, _expression.Length - index - 2);

                    var op_end_index = _expression.LastIndexOf(' ', index);
                    var op_start_index = _expression.LastIndexOf(' ', op_end_index - 1) + 1;
                    var op = _expression.Substring(op_start_index, 1);

                    var left_expression = _expression.Substring(0, op_start_index - 1);

                    if (op == "+") {
                        return EvaluateExpression(left_expression) + EvaluateExpression(right_expression);
                    } else {
                        return EvaluateExpression(left_expression) * EvaluateExpression(right_expression);
                    }
                }
            } else if (_expression.Contains(' ')) {
                var num_start_index = _expression.LastIndexOf(' ');
                var op_start_index = _expression.LastIndexOf(' ', num_start_index - 1);

                var num = Convert.ToInt32(_expression.Substring(num_start_index + 1));
                var op = _expression.Substring(op_start_index + 1, 1);

                if (op == "+") {
                    return num + EvaluateExpression(_expression.Substring(0, op_start_index));
                } else {
                    return num * EvaluateExpression(_expression.Substring(0, op_start_index));
                }
            } else {
                return Convert.ToInt32(_expression);
            }
        }
    }
}