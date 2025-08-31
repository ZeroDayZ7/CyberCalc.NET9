using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WPF_CALC_NET_9.ViewModels;

namespace WPF_CALC_NET_9.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        // Modern dictionary approach for key mappings
        private readonly Dictionary<Key, Func<bool, string?>> _keyMappings = new()
        {
            // Numbers - NumPad
            [Key.NumPad0] = _ => "0",
            [Key.NumPad1] = _ => "1",
            [Key.NumPad2] = _ => "2",
            [Key.NumPad3] = _ => "3",
            [Key.NumPad4] = _ => "4",
            [Key.NumPad5] = _ => "5",
            [Key.NumPad6] = _ => "6",
            [Key.NumPad7] = _ => "7",
            [Key.NumPad8] = _ => "8",
            [Key.NumPad9] = _ => "9",

            // Numbers - Main keyboard with shift handling
            [Key.D0] = shift => shift ? ")" : "0",
            [Key.D1] = _ => "1",
            [Key.D2] = _ => "2",
            [Key.D3] = _ => "3",
            [Key.D4] = _ => "4",
            [Key.D5] = shift => shift ? "%" : "5",
            [Key.D6] = shift => shift ? "^" : "6",
            [Key.D7] = _ => "7",
            [Key.D8] = shift => shift ? "*" : "8",
            [Key.D9] = shift => shift ? "(" : "9",

            // Operators
            [Key.Add] = _ => "+",
            [Key.OemPlus] = _ => "+",
            [Key.Subtract] = _ => "-",
            [Key.OemMinus] = _ => "-",
            [Key.Multiply] = _ => "*",
            [Key.Divide] = _ => "/",
            [Key.OemQuestion] = _ => "/",

            // Decimal separator
            [Key.OemComma] = _ => ",",
            [Key.Decimal] = _ => ",",
            [Key.OemPeriod] = _ => ",",

            // Parentheses
            [Key.OemOpenBrackets] = _ => "(",
            [Key.OemCloseBrackets] = _ => ")"
        };

        private readonly Dictionary<Key, Func<bool, string?>> _functionMappings = new()
        {
            [Key.S] = shift => shift ? "sin" : null,
            [Key.C] = shift => shift ? "cos" : null,
            [Key.T] = shift => shift ? "tan" : null,
            [Key.L] = shift => shift ? "log" : null,
            [Key.Q] = shift => shift ? "sqrt" : null
        };

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            try
            {
                // Handle special keys first
                switch (e.Key)
                {
                    case Key.Enter:
                        _viewModel.CalculateCommand.Execute(null);
                        e.Handled = true;
                        return;

                    case Key.Delete:
                    case Key.Back:
                        _viewModel.ClearEntryCommand.Execute(null);
                        e.Handled = true;
                        return;

                    case Key.Escape:
                        _viewModel.ClearCommand.Execute(null);
                        e.Handled = true;
                        return;
                }

                // Handle function keys (with Shift)
                if (shiftPressed && _functionMappings.TryGetValue(e.Key, out var functionMapping))
                {
                    var function = functionMapping(shiftPressed);
                    if (!string.IsNullOrEmpty(function))
                    {
                        _viewModel.ProcessFunction(function);
                        e.Handled = true;
                        return;
                    }
                }

                // Handle regular input mappings
                if (_keyMappings.TryGetValue(e.Key, out var mapping))
                {
                    var input = mapping(shiftPressed);
                    if (!string.IsNullOrEmpty(input))
                    {
                        _viewModel.ProcessInput(input);
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or show user-friendly message
                MessageBox.Show($"An error occurred while processing input: {ex.Message}",
                    "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            // Dispose()
        }
    }
}