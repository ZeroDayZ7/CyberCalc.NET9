using System.Data;

namespace WPF_CALC_NET_9.Models;

public class CalculatorModel
{
    public static string Calculate(string expression)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(expression))
                return "0";

            if (expression.Contains('%'))
            {
                return HandlePercentage(expression);
            }
            else if (expression.Contains('^'))
            {
                return HandlePower(expression);
            }

            if (expression.Contains("/0"))
            {
                return "Błąd: Dzielenie przez zero";
            }

            DataTable dt = new();
            var result = dt.Compute(expression, "");

            if (result == DBNull.Value)
                return "Błąd obliczenia";

            double numResult = Convert.ToDouble(result);

            if (numResult % 1 == 0 && numResult >= int.MinValue && numResult <= int.MaxValue)
                return ((int)numResult).ToString();
            else
                return Math.Round(numResult, 10).ToString();
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

    private static string HandlePercentage(string expression)
    {
        try
        {
            if (string.IsNullOrEmpty(expression))
                return "0";

            switch (true)
            {
                case var _ when expression.Contains('+'):
                    var partsAdd = expression.Split('+');
                    if (partsAdd.Length > 1 && partsAdd[1].Contains('%'))
                    {
                        double numberPart = double.Parse(partsAdd[0].Trim());
                        string secondPart = partsAdd[1].Replace("%", "").Trim();
                        double percentValue = double.Parse(secondPart) / 100;
                        double addResult = numberPart + (numberPart * percentValue);
                        return addResult.ToString();
                    }
                    break;
                case var _ when expression.Contains('-'):
                    var partsSub = expression.Split('-');
                    if (partsSub.Length > 1 && partsSub[1].Contains('%'))
                    {
                        double numberPart = double.Parse(partsSub[0].Trim());
                        double percentValue = double.Parse(partsSub[1].Replace("%", "").Trim()) / 100.0;
                        double subResult = numberPart - (numberPart * percentValue);
                        return subResult.ToString();
                    }
                    break;
                case var _ when expression.Contains('*'):
                    expression = expression.Replace("%", "/100");
                    DataTable dt = new();
                    var mulResult = dt.Compute(expression, "");
                    if (mulResult == DBNull.Value || mulResult == null)
                        return "Błąd obliczenia";
                    return mulResult.ToString() ?? "0"; // Zabezpieczenie przed null
                case var _ when expression.Contains('/'):
                    var partsDiv = expression.Split('/');
                    if (partsDiv.Length > 1 && partsDiv[1].Contains('%'))
                    {
                        double dividend = double.Parse(partsDiv[0].Trim());
                        double divisor = double.Parse(partsDiv[1].Replace("%", "").Trim()) / 100.0;
                        if (divisor != 0)
                        {
                            double divResult = dividend / divisor;
                            return divResult.ToString();
                        }
                        else
                        {
                            throw new DivideByZeroException("Dzielenie przez zero");
                        }
                    }
                    break;
            }
            return "0"; // Zwracamy "0" zamiast expression, aby uniknąć ostrzeżenia CS8603
        }
        catch (Exception ex)
        {
            return "Błąd: " + ex.Message;
        }
    }

    private static string HandlePower(string expression)
    {
        try
        {
            if (string.IsNullOrEmpty(expression))
                return "0";

            string[] parts = expression.Split('^');
            if (parts.Length == 2)
            {
                double baseNumber = double.Parse(parts[0].Trim());
                double exponent = double.Parse(parts[1].Trim());
                double powResult = Math.Pow(baseNumber, exponent);
                return powResult.ToString();
            }
            return "0"; // Zwracamy "0" zamiast expression
        }
        catch (Exception ex)
        {
            return "Błąd: " + ex.Message;
        }
    }
}