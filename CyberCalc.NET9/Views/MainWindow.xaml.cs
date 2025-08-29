using System.Windows;
using System.Windows.Input;
using WPF_CALC_NET_9.ViewModels;

namespace WPF_CALC_NET_9.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool shiftPressed = Keyboard.Modifiers == ModifierKeys.Shift;

            switch (e.Key)
            {
                // Cyfry z NumPad
                case Key.NumPad0: viewModel.ProcessNumber("0"); break;
                case Key.NumPad1: viewModel.ProcessNumber("1"); break;
                case Key.NumPad2: viewModel.ProcessNumber("2"); break;
                case Key.NumPad3: viewModel.ProcessNumber("3"); break;
                case Key.NumPad4: viewModel.ProcessNumber("4"); break;
                case Key.NumPad5: viewModel.ProcessNumber("5"); break;
                case Key.NumPad6: viewModel.ProcessNumber("6"); break;
                case Key.NumPad7: viewModel.ProcessNumber("7"); break;
                case Key.NumPad8: viewModel.ProcessNumber("8"); break;
                case Key.NumPad9: viewModel.ProcessNumber("9"); break;

                // Cyfry z głównej klawiatury
                case Key.D0: viewModel.ProcessInput(shiftPressed ? ")" : "0"); break;
                case Key.D1: viewModel.ProcessNumber("1"); break;
                case Key.D2: viewModel.ProcessNumber("2"); break;
                case Key.D3: viewModel.ProcessNumber("3"); break;
                case Key.D4: viewModel.ProcessNumber("4"); break;
                case Key.D5: viewModel.ProcessInput(shiftPressed ? "%" : "5"); break;
                case Key.D6: viewModel.ProcessInput(shiftPressed ? "^" : "6"); break;
                case Key.D7: viewModel.ProcessNumber("7"); break;
                case Key.D8: viewModel.ProcessInput(shiftPressed ? "*" : "8"); break;
                case Key.D9: viewModel.ProcessInput(shiftPressed ? "(" : "9"); break;

                // Operatory
                case Key.Add:
                case Key.OemPlus: viewModel.ProcessInput("+"); break;
                case Key.Subtract:
                case Key.OemMinus: viewModel.ProcessInput("-"); break;
                case Key.Multiply: viewModel.ProcessInput("*"); break;
                case Key.Divide:
                case Key.OemQuestion: viewModel.ProcessInput("/"); break;
                case Key.OemComma:
                case Key.Decimal:
                case Key.OemPeriod: viewModel.ProcessInput(","); break;

                // Nawiasy
                case Key.OemOpenBrackets: viewModel.ProcessInput("("); break;
                case Key.OemCloseBrackets: viewModel.ProcessInput(")"); break;

                // Enter do obliczeń
                case Key.Enter: viewModel.CalculateCommand.Execute(null); break;

                // Czyszczenie
                case Key.Delete:
                case Key.Back: viewModel.ClearEntryCommand.Execute(null); break;
                case Key.Escape: viewModel.ClearCommand.Execute(null); break;

                // Funkcje
                case Key.S: if (shiftPressed) viewModel.ProcessFunction("sin"); break;
                case Key.C: if (shiftPressed) viewModel.ProcessFunction("cos"); break;
                case Key.T: if (shiftPressed) viewModel.ProcessFunction("tan"); break;
                case Key.L: if (shiftPressed) viewModel.ProcessFunction("log"); break;
                case Key.Q: if (shiftPressed) viewModel.ProcessFunction("sqrt"); break;
            }

            e.Handled = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
