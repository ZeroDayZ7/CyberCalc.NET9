using System;

namespace WPF_CALC_NET_9.Models
{
    public enum TokenType
    {
        Number,
        Operator,
        Function,
        LeftParen,
        RightParen,
        Variable
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int Precedence { get; }
        public bool IsRightAssociative { get; }

        public Token(TokenType type, string value, int precedence = 0, bool isRightAssociative = false)
        {
            Type = type;
            Value = value;
            Precedence = precedence;
            IsRightAssociative = isRightAssociative;
        }

        public override string ToString() => $"{Type}: {Value}";
    }
}