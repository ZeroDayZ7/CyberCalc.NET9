var tokenizer = new Tokenizer();
var shuntingYard = new ShuntingYard();
var evaluator = new MathEvaluator();

string expression = "2 + 3 * sin(0.5)";
var tokens = tokenizer.Tokenize(expression);
var rpn = shuntingYard.ToRPN(tokens);
var result = evaluator.Evaluate(rpn);
Console.WriteLine($"Wynik: {result}");