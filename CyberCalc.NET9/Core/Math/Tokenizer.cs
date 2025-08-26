using System;
using System.Collections.Generic;
using System.Globalization;

namespace WPF_CALC_NET_9.Core.Math
{
    public class Tokenizer
    {
        private readonly Dictionary<string, (int precedence, bool isRightAssociative)> _operators = new()
        {
            { "+", (1, false) },
            { "-", (1, false) },
            { "*", (2, false) },
            { "/", (2, false) },
            { "^", (3, true) },
            { "%", (2, false) }
        };

        private readonly HashSet<string> _functions = new() { "sin", "cos", "tan", "log", "sqrt" };

        public List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            input = input.Replace(" ", "").ToLower();
            int i = 0;

            while (i < input.Length)
            {
                char c = input[i];

                // Liczby
                if (char.IsDigit(c) || c == '.')
                {
                    string number = "";
                    while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
                        number += input[i++];
                    tokens.Add(new Token(TokenType.Number, number));
                    continue;
                }

                // Operatory
                if (_operators.ContainsKey(c.ToString()))
                {
                    tokens.Add(new Token(TokenType.Operator, c.ToString(), _operators[c.ToString()].precedence, _operators[c.ToString()].isRightAssociative));
                    i++;
                    continue;
                }

                // Funkcje i zmienne
                if (char.IsLetter(c))
                {
                    string func = "";
                    while (i < input.Length && char.IsLetter(input[i]))
                        func += input[i++];
                    if (_functions.Contains(func))
                        tokens.Add(new Token(TokenType.Function, func));
                    else
                        tokens.Add(new Token(TokenType.Variable, func));
                    continue;
                }

                // Nawiasy
                if (c == '(') tokens.Add(new Token(TokenType.LeftParen, "("));
                else if (c == ')') tokens.Add(new Token(TokenType.RightParen, ")"));
                else throw new ArgumentException($"Nieznany znak: {c}");

                i++;
            }

            return tokens;
        }
    }
}