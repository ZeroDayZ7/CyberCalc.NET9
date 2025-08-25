using System.Windows;
using System.Windows.Input;
using WPF_CALC_NET_9.ViewModels;

namespace WPF_CALC_NET_9.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
                Focus();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            var viewModel = (MainViewModel)DataContext;
            bool shiftPressed = Keyboard.Modifiers == ModifierKeys.Shift;

            switch (e.Key)
            {
                case Key.NumPad0: viewModel.NumberCommand.Execute("0"); break;
                case Key.NumPad1: viewModel.NumberCommand.Execute("1"); break;
                case Key.NumPad2: viewModel.NumberCommand.Execute("2"); break;
                case Key.NumPad3: viewModel.NumberCommand.Execute("3"); break;
                case Key.NumPad4: viewModel.NumberCommand.Execute("4"); break;
                case Key.NumPad5: viewModel.NumberCommand.Execute("5"); break;
                case Key.NumPad6: viewModel.NumberCommand.Execute("6"); break;
                case Key.NumPad7: viewModel.NumberCommand.Execute("7"); break;
                case Key.NumPad8: viewModel.NumberCommand.Execute("8"); break;
                case Key.NumPad9: viewModel.NumberCommand.Execute("9"); break;
                case Key.D0:
                    viewModel.NumberCommand.Execute(shiftPressed ? ")" : "0");
                    break;
                case Key.D1: viewModel.NumberCommand.Execute("1"); break;
                case Key.D2: viewModel.NumberCommand.Execute("2"); break;
                case Key.D3: viewModel.NumberCommand.Execute("3"); break;
                case Key.D4: viewModel.NumberCommand.Execute("4"); break;
                case Key.D5:
                    viewModel.OperatorCommand.Execute(shiftPressed ? "%" : "5");
                    break;
                case Key.D6:
                    viewModel.OperatorCommand.Execute(shiftPressed ? "^" : "6");
                    break;
                case Key.D7: viewModel.NumberCommand.Execute("7"); break;
                case Key.D8:
                    viewModel.OperatorCommand.Execute(shiftPressed ? "*" : "8");
                    break;
                case Key.D9:
                    viewModel.OperatorCommand.Execute(shiftPressed ? "(" : "9");
                    break;
                case Key.Add: viewModel.OperatorCommand.Execute("+"); break;
                case Key.OemPlus:
                    if (shiftPressed) viewModel.OperatorCommand.Execute("+");
                    else viewModel.CalculateCommand.Execute(null);
                    break;
                case Key.Subtract: viewModel.OperatorCommand.Execute("-"); break;
                case Key.OemMinus: viewModel.OperatorCommand.Execute("-"); break;
                case Key.Multiply: viewModel.OperatorCommand.Execute("*"); break;
                case Key.Divide: viewModel.OperatorCommand.Execute("/"); break;
                case Key.OemQuestion:
                    if (!shiftPressed) viewModel.OperatorCommand.Execute("/");
                    break;
                case Key.OemComma: viewModel.OperatorCommand.Execute(","); break;
                case Key.Decimal: viewModel.OperatorCommand.Execute(","); break;
                case Key.OemPeriod: viewModel.OperatorCommand.Execute(","); break;
                case Key.Enter: viewModel.CalculateCommand.Execute(null); break;
                case Key.Delete: case Key.Back: viewModel.ClearEntryCommand.Execute(null); break;
                case Key.Escape: viewModel.ClearCommand.Execute(null); break;
                // Dodano obsługę funkcji
                case Key.S: if (shiftPressed) viewModel.FunctionCommand.Execute("sin"); break;
                case Key.C: if (shiftPressed) viewModel.FunctionCommand.Execute("cos"); break;
                case Key.T: if (shiftPressed) viewModel.FunctionCommand.Execute("tan"); break;
                case Key.L: if (shiftPressed) viewModel.FunctionCommand.Execute("log"); break;
                case Key.Q: if (shiftPressed) viewModel.FunctionCommand.Execute("sqrt"); break;
            }
            e.Handled = true;
        }
    }
}