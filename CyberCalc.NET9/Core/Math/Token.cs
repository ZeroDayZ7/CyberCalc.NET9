using System;

namespace WPF_CALC_NET_9.Core.Math
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

    public class Token(TokenType type, string value, int precedence = 0, bool isRightAssociative = false)
    {
        public TokenType Type { get; } = type;
        public string Value { get; } = value;
        public int Precedence { get; } = precedence;
        public bool IsRightAssociative { get; } = isRightAssociative;

        public override string ToString() => $"{Type}: {Value}";
    }

}