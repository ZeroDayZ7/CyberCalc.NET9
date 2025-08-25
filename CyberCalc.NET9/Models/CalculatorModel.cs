using System;
using System.Globalization;

namespace WPF_CALC_NET_9.Models
{
    public class CalculatorModel
    {
        private readonly Tokenizer _tokenizer;
        private readonly ShuntingYard _shuntingYard;
        private readonly IEvaluator _evaluator;

        public CalculatorModel()
        {
            _tokenizer = new Tokenizer();
            _shuntingYard = new ShuntingYard();
            _evaluator = new MathEvaluator();
        }

        public string Calculate(string expression)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                    return "0";

                // Zamień przecinek na kropkę dla spójności
                expression = expression.Replace(',', '.');

                // Tokenizacja
                var tokens = _tokenizer.Tokenize(expression);

                // Konwersja na ONP
                var rpnTokens = _shuntingYard.ToRPN(tokens);

                // Ewaluacja
                var result = _evaluator.Evaluate(rpnTokens);

                // Formatowanie wyniku
                if (Math.Abs(result % 1) < 1e-10 && result >= int.MinValue && result <= int.MaxValue)
                    return ((int)result).ToString(CultureInfo.InvariantCulture);
                return Math.Round(result, 10).ToString(CultureInfo.InvariantCulture);
            }
            catch (DivideByZeroException)
            {
                return "Błąd: Dzielenie przez zero";
            }
            catch (Exception ex)
            {
                return "Błąd: " + ex.Message;
            }
        }
    }
}