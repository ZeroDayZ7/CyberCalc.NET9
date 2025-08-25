using System.Collections.Generic;

namespace WPF_CALC_NET_9.Models
{
    public interface IEvaluator
    {
        double Evaluate(List<Token> rpnTokens);
    }
}