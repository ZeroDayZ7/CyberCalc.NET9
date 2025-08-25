using System;
using System.Collections.Generic;

namespace WPF_CALC_NET_9.Models
{
    public class ShuntingYard
    {
        public List<Token> ToRPN(List<Token> tokens)
        {
            var output = new List<Token>();
            var operators = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                    case TokenType.Variable:
                        output.Add(token);
                        break;

                    case TokenType.Function:
                        operators.Push(token);
                        break;

                    case TokenType.LeftParen:
                        operators.Push(token);
                        break;

                    case TokenType.RightParen:
                        while (operators.Count > 0 && operators.Peek().Type != TokenType.LeftParen)
                            output.Add(operators.Pop());
                        if (operators.Count == 0) throw new ArgumentException("Niezamknięty nawias");
                        operators.Pop(); // Usuń '('
                        if (operators.Count > 0 && operators.Peek().Type == TokenType.Function)
                            output.Add(operators.Pop());
                        break;

                    case TokenType.Operator:
                        while (operators.Count > 0 && operators.Peek().Type == TokenType.Operator &&
                               ((token.IsRightAssociative && token.Precedence < operators.Peek().Precedence) ||
                                (!token.IsRightAssociative && token.Precedence <= operators.Peek().Precedence)))
                            output.Add(operators.Pop());
                        operators.Push(token);
                        break;
                }
            }

            while (operators.Count > 0)
            {
                if (operators.Peek().Type == TokenType.LeftParen)
                    throw new ArgumentException("Niezamknięty nawias");
                output.Add(operators.Pop());
            }

            return output;
        }
    }
}