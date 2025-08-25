using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WPF_CALC_NET_9.Helpers;
using WPF_CALC_NET_9.Models;

namespace WPF_CALC_NET_9.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _calculator;
        private string _resultText = "0";
        private string _historyText = "";

        public MainViewModel()
        {
            _calculator = new CalculatorModel();
            LoadLastResults();
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
            FunctionCommand = new RelayCommand(ProcessFunction); // Nowy command dla funkcji
        }

        public string ResultText
        {
            get => _resultText;
            set
            {
                _resultText = value;
                OnPropertyChanged();
            }
        }

        public string HistoryText
        {
            get => _historyText;
            set
            {
                _historyText = value;
                OnPropertyChanged();
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ThemeCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ClearHistoryCommand { get; }
        public ICommand FunctionCommand { get; } // Nowy command

        private void ClearHistory(object? parameter)
        {
            try
            {
                if (File.Exists("wyniki.txt"))
                {
                    File.WriteAllText("wyniki.txt", string.Empty);
                }
                HistoryText = string.Empty;
                MessageBox.Show("Historia została wyczyszczona.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas czyszczenia historii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessNumber(object? parameter)
        {
            var input = parameter?.ToString() ?? "";
            switch (input)
            {
                case "0" when ResultText == "0":
                    return;
                default:
                    ResultText = ResultText == "0" ? input : ResultText + input;
                    break;
            }
        }

        private void ProcessOperator(object? parameter)
        {
            var input = parameter?.ToString() ?? "";
            switch (input)
            {
                case "+" or "-" or "*" or "/" or "%" or "^" when !string.IsNullOrEmpty(ResultText) && !IsOperatorOrParen(ResultText.Last().ToString()):
                    ResultText += input;
                    break;
                case "," when !string.IsNullOrEmpty(ResultText) && ResultText.Last() != ',':
                    var parts = ResultText.Split('+', '-', '*', '/', '%', '^', '(').LastOrDefault() ?? "";
                    if (!parts.Contains(','))
                    {
                        ResultText += input;
                    }
                    break;
                case "(" or ")":
                    ResultText += input;
                    break;
            }
        }

        private void ProcessFunction(object? parameter)
        {
            var input = parameter?.ToString() ?? "";
            if (!string.IsNullOrEmpty(input))
            {
                ResultText = ResultText == "0" ? input + "(" : ResultText + input + "(";
            }
        }

        private static bool IsOperatorOrParen(string str) => str is "+" or "-" or "*" or "/" or "%" or "^" or "(" or ")";

        private void Clear(object? parameter)
        {
            ResultText = "0";
        }

        private void ClearEntry(object? parameter)
        {
            if (ResultText.Length > 1)
            {
                ResultText = ResultText.Substring(0, ResultText.Length - 1);
            }
            else
            {
                ResultText = "0";
            }
        }

        private void Calculate(object? parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ResultText)) return;
                var expression = ResultText.Replace(',', '.');
                var result = _calculator.Calculate(expression);
                SaveResult($"{ResultText} = {result}");
                ResultText = result;
                LoadLastResults();
            }
            catch (Exception ex)
            {
                ResultText = "Błąd: " + ex.Message;
            }
        }

        private static void SaveResult(string result)
        {
            try
            {
                File.AppendAllText("wyniki.txt", result + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd zapisu: " + ex.Message);
            }
        }

        private void LoadLastResults()
        {
            try
            {
                if (File.Exists("wyniki.txt"))
                {
                    var allLines = File.ReadAllLines("wyniki.txt");
                    Array.Reverse(allLines);
                    var lastFiveLines = allLines.Take(6).ToArray();
                    HistoryText = string.Join(Environment.NewLine, lastFiveLines);
                }
            }
            catch (Exception ex)
            {
                HistoryText = "Błąd wczytywania historii: " + ex.Message;
            }
        }

        private void ChangeTheme(object? parameter)
        {
            ThemeManager.ApplyTheme(parameter?.ToString() ?? "Cyberpunk");
        }

        private void ShowSettings(object? parameter)
        {
            var settingsWindow = new Views.Settings { Owner = Application.Current.MainWindow };
            settingsWindow.ShowDialog();
        }

        private void ShowAbout(object? parameter)
        {
            var aboutWindow = new Views.AboutWindow { Owner = Application.Current.MainWindow };
            aboutWindow.ShowDialog();
        }

        private void Minimize(object? parameter)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Close(object? parameter)
        {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}