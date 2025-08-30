using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_CALC_NET_9.Helpers;
using WPF_CALC_NET_9.Models;

namespace WPF_CALC_NET_9.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _calculator;
        private string _resultText = "0";
        private string _historyText = "";

        // Constants for better maintainability
        private const string DefaultValue = "0";
        private const string HistoryFileName = "calculation_history.txt";
        private const string Operators = "+-*/%^";
        private const string OpenParentheses = "(";
        private const string CloseParentheses = ")";
        private const string DecimalSeparator = ",";
        private const int MaxHistoryLines = 6;

        public MainViewModel()
        {
            _calculator = new CalculatorModel();
            InitializeCommands();
            _ = LoadCalculationHistoryAsync(); // Fire and forget async initialization
        }

        #region Properties
        public string ResultText
        {
            get => _resultText;
            set => SetProperty(ref _resultText, value);
        }

        public string HistoryText
        {
            get => _historyText;
            set => SetProperty(ref _historyText, value);
        }
        #endregion

        #region Commands
        public ICommand NumberCommand { get; private set; } = null!;
        public ICommand OperatorCommand { get; private set; } = null!;
        public ICommand ClearCommand { get; private set; } = null!;
        public ICommand ClearEntryCommand { get; private set; } = null!;
        public ICommand CalculateCommand { get; private set; } = null!;
        public ICommand ThemeCommand { get; private set; } = null!;
        public ICommand ShowSettingsCommand { get; private set; } = null!;
        public ICommand ShowAboutCommand { get; private set; } = null!;
        public ICommand MinimizeCommand { get; private set; } = null!;
        public ICommand CloseCommand { get; private set; } = null!;
        public ICommand ClearHistoryCommand { get; private set; } = null!;
        public ICommand FunctionCommand { get; private set; } = null!;
        #endregion

        #region Command Initialization
        private void InitializeCommands()
        {
            NumberCommand = new RelayCommand(ProcessNumber);
            OperatorCommand = new RelayCommand(ProcessOperator);
            ClearCommand = new RelayCommand(Clear);
            ClearEntryCommand = new RelayCommand(ClearEntry);
            CalculateCommand = new RelayCommand(Calculate);
            ThemeCommand = new RelayCommand(ChangeTheme);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ShowAboutCommand = new RelayCommand(ShowAbout);
            MinimizeCommand = new RelayCommand(Minimize);
            CloseCommand = new RelayCommand(Close);
            ClearHistoryCommand = new RelayCommand(ClearHistory);
            FunctionCommand = new RelayCommand(ProcessFunction);
        }
        #endregion

        #region Input Processing - Improved and Unified
        public void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            // Handle different input types through a cleaner switch expression
            _ = input switch
            {
                var digit when char.IsDigit(digit[0]) => ProcessNumberInput(digit),
                var op when Operators.Contains(op) => ProcessOperatorInput(op),
                OpenParentheses => ProcessOpenParenthesis(),
                CloseParentheses => ProcessCloseParenthesis(),
                DecimalSeparator => ProcessDecimalSeparator(),
                var func when IsValidFunction(func) => ProcessFunctionInput(func),
                _ => false // Unknown input, ignore
            };
        }

        private bool ProcessNumberInput(string digit)
        {
            ResultText = ResultText == DefaultValue ? digit : ResultText + digit;
            return true;
        }

        private bool ProcessOperatorInput(string operatorChar)
        {
            if (string.IsNullOrEmpty(ResultText)) return false;

            var lastChar = ResultText[^1];

            // Don't insert operator at the beginning or after opening parenthesis
            if (ResultText.Length == 0 || lastChar == '(') return false;

            // Replace last operator if the last character is an operator
            if (Operators.Contains(lastChar))
            {
                ResultText = ResultText[..^1] + operatorChar;
            }
            else
            {
                ResultText += operatorChar;
            }
            return true;
        }

        private bool ProcessOpenParenthesis()
        {
            var lastChar = ResultText.Length > 0 ? ResultText[^1] : '\0';

            // Add multiplication before parenthesis if needed
            if (char.IsDigit(lastChar) || lastChar == ')')
            {
                ResultText += "*" + OpenParentheses;
            }
            else
            {
                ResultText += OpenParentheses;
            }
            return true;
        }

        private bool ProcessCloseParenthesis()
        {
            var openCount = ResultText.Count(c => c == '(');
            var closeCount = ResultText.Count(c => c == ')');
            var lastChar = ResultText.Length > 0 ? ResultText[^1] : '\0';

            // Only add closing parenthesis if there are unmatched opening ones
            // and the last character is not an operator or opening parenthesis
            if (openCount > closeCount && !$" {Operators}(".Contains(lastChar))
            {
                ResultText += CloseParentheses;
                return true;
            }
            return false;
        }

        private bool ProcessDecimalSeparator()
        {
            if (string.IsNullOrEmpty(ResultText))
            {
                ResultText = "0" + DecimalSeparator;
                return true;
            }

            var lastChar = ResultText[^1];

            // Don't add decimal if last character is already a decimal
            if (lastChar == ',') return false;

            // Get the current number (last part after any operator or parenthesis)
            var operators = new char[] { '+', '-', '*', '/', '%', '^', '(' };
            var lastOperatorIndex = -1;

            for (int i = ResultText.Length - 1; i >= 0; i--)
            {
                if (operators.Contains(ResultText[i]))
                {
                    lastOperatorIndex = i;
                    break;
                }
            }

            var currentNumber = lastOperatorIndex >= 0
                ? ResultText[(lastOperatorIndex + 1)..]
                : ResultText;

            // Only add decimal separator if current number doesn't already have one
            if (!currentNumber.Contains(DecimalSeparator))
            {
                // If we're at the start or after an operator, add "0,"
                if (ResultText == DefaultValue || Operators.Contains(lastChar) || lastChar == '(')
                {
                    ResultText = ResultText == DefaultValue ? "0" + DecimalSeparator : ResultText + "0" + DecimalSeparator;
                }
                else
                {
                    ResultText += DecimalSeparator;
                }
                return true;
            }
            return false;
        }

        private bool ProcessFunctionInput(string function)
        {
            var lastChar = ResultText.Length > 0 ? ResultText[^1] : '\0';

            // Add multiplication before function if needed
            if (char.IsDigit(lastChar) || lastChar == ')')
            {
                ResultText += "*" + function + OpenParentheses;
            }
            else
            {
                ResultText = ResultText == DefaultValue ? function + OpenParentheses : ResultText + function + OpenParentheses;
            }
            return true;
        }

        private static bool IsValidFunction(string input) =>
            input is "sin" or "cos" or "tan" or "log" or "sqrt" or "ln" or "abs";
        #endregion

        #region Command Handlers - Simplified
        private void ProcessNumber(object? parameter)
        {
            var input = parameter?.ToString();
            if (string.IsNullOrEmpty(input)) return;

            ProcessNumberInput(input);
        }

        private void ProcessOperator(object? parameter)
        {
            var input = parameter?.ToString();
            if (string.IsNullOrEmpty(input)) return;

            // Handle decimal separator with improved logic
            if (input == DecimalSeparator)
            {
                ProcessDecimalSeparator();
                return;
            }

            // Handle parentheses
            if (input is OpenParentheses or CloseParentheses)
            {
                if (input == OpenParentheses)
                    ProcessOpenParenthesis();
                else
                    ProcessCloseParenthesis();
                return;
            }

            // Handle operators
            if (Operators.Contains(input))
            {
                ProcessOperatorInput(input);
            }
        }

        public void ProcessFunction(object? parameter)
        {
            var input = parameter?.ToString();
            if (string.IsNullOrEmpty(input)) return;

            ProcessFunctionInput(input);
        }

        private void Clear(object? parameter) => ResultText = DefaultValue;

        private void ClearEntry(object? parameter)
        {
            ResultText = ResultText.Length > 1 ? ResultText[..^1] : DefaultValue;
        }

        private async void Calculate(object? parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ResultText)) return;

                var expression = ResultText.Replace(DecimalSeparator, ".");
                var result = _calculator.Calculate(expression);

                // Formatuj wynik, zamieniając kropkę na przecinek
                string formattedResult = result.Replace(".", DecimalSeparator);

                await SaveCalculationAsync($"{ResultText} = {formattedResult}");
                ResultText = formattedResult;
                await LoadCalculationHistoryAsync();
            }
            catch (Exception ex)
            {
                ResultText = $"Error: {ex.Message}";
            }
        }

        private async void ClearHistory(object? parameter)
        {
            try
            {
                if (File.Exists(HistoryFileName))
                {
                    await File.WriteAllTextAsync(HistoryFileName, string.Empty);
                }
                HistoryText = string.Empty;

                MessageBox.Show("Calculation history has been cleared.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while clearing history: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region File Operations - Async for better performance
        private static async Task SaveCalculationAsync(string result)
        {
            try
            {
                await File.AppendAllTextAsync(HistoryFileName, result + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving history: {ex.Message}");
            }
        }

        private async Task LoadCalculationHistoryAsync()
        {
            try
            {
                if (!File.Exists(HistoryFileName))
                {
                    HistoryText = string.Empty;
                    return;
                }

                var allLines = await File.ReadAllLinesAsync(HistoryFileName);
                var lastLines = allLines
                    .Reverse()
                    .Take(MaxHistoryLines)
                    .ToArray();

                HistoryText = string.Join(Environment.NewLine, lastLines);
            }
            catch (Exception ex)
            {
                HistoryText = $"Error loading history: {ex.Message}";
            }
        }
        #endregion

        #region Window Operations
        private static void ChangeTheme(object? parameter)
        {
            ThemeManager.ApplyTheme(parameter?.ToString() ?? "Cyberpunk");
        }

        private static void ShowSettings(object? parameter)
        {
            var settingsWindow = new Views.Settings { Owner = Application.Current.MainWindow };
            settingsWindow.ShowDialog();
        }

        private static void ShowAbout(object? parameter)
        {
            var aboutWindow = new Views.AboutWindow { Owner = Application.Current.MainWindow };
            aboutWindow.ShowDialog();
        }

        private static void Minimize(object? parameter)
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private static void Close(object? parameter)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region INotifyPropertyChanged - Modern approach
        public event PropertyChangedEventHandler? PropertyChanged;

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;

            field = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    // Modern RelayCommand using primary constructor (C# 12) with improved null safety
    public sealed class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
    {
        private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<object?, bool>? _canExecute = canExecute;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    // Extension methods for better code organization
    public static class StringExtensions
    {
        public static bool ContainsAny(this string source, params char[] characters)
        {
            return characters.Any(source.Contains);
        }

        public static string GetLastNumberPart(this string expression)
        {
            if (string.IsNullOrEmpty(expression)) return string.Empty;

            var operators = new[] { '+', '-', '*', '/', '%', '^', '(' };
            var lastOperatorIndex = -1;

            for (int i = expression.Length - 1; i >= 0; i--)
            {
                if (operators.Contains(expression[i]))
                {
                    lastOperatorIndex = i;
                    break;
                }
            }

            return lastOperatorIndex >= 0
                ? expression[(lastOperatorIndex + 1)..]
                : expression;
        }
    }
}