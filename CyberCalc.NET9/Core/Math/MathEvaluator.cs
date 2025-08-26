using System;
using System.Collections.Generic;
using System.Globalization;
using SysMath = System.Math;


namespace WPF_CALC_NET_9.Core.Math

{
    public class MathEvaluator : IEvaluator
    {
        private readonly Dictionary<string, Func<double, double, double>> _binaryOperators = new()
        {
            { "+", (a, b) => a + b },
            { "-", (a, b) => a - b },
            { "*", (a, b) => a * b },
            { "/", (a, b) => b == 0 ? throw new DivideByZeroException() : a / b },
            { "^", (a, b) => SysMath.Pow(a, b) }
        };

        private readonly Dictionary<string, Func<double, double>> _functions = new()
        {
            { "sin", SysMath.Sin },
            { "cos", SysMath.Cos },
            { "tan", SysMath.Tan },
            { "log", SysMath.Log },
            { "sqrt", SysMath.Sqrt }
        };

        public double Evaluate(List<Token> rpnTokens)
        {
            var stack = new Stack<double>();

            foreach (var token in rpnTokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        stack.Push(double.Parse(token.Value, CultureInfo.InvariantCulture));
                        break;

                    case TokenType.Operator:
                        if (token.Value == "%")
                        {
                            if (stack.Count < 1) throw new ArgumentException("Za mało operandów dla %");
                            var percentage = stack.Pop(); // Wartość procentowa (np. 10 dla 10%)
                            double result = percentage / 100.0; // Przekształć na ułamek (np. 10 / 100 = 0.1)
                            if (stack.Count >= 1)
                            {
                                // Jeśli jest poprzedni operand, pomnóż go przez procent
                                var baseValue = stack.Peek(); // Poprzednia liczba (np. 1460)
                                result = baseValue * (percentage / 100.0); // np. 1460 * 0.1 = 146
                            }
                            stack.Push(result);
                        }
                        else if (_binaryOperators.ContainsKey(token.Value))
                        {
                            if (stack.Count < 2) throw new ArgumentException("Za mało operandów");
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push(_binaryOperators[token.Value](a, b));
                        }
                        else
                            throw new ArgumentException($"Nieznany operator: {token.Value}");
                        break;

                    case TokenType.Function:
                        if (_functions.ContainsKey(token.Value))
                        {
                            if (stack.Count < 1) throw new ArgumentException("Za mało argumentów dla funkcji");
                            var arg = stack.Pop();
                            stack.Push(_functions[token.Value](arg));
                        }
                        else
                            throw new ArgumentException($"Nieznana funkcja: {token.Value}");
                        break;

                    case TokenType.Variable:
                        throw new NotSupportedException("Zmienne nie są jeszcze obsługiwane");
                }
            }

            if (stack.Count != 1) throw new ArgumentException("Błędne wyrażenie");
            return stack.Pop();
        }
  }
}