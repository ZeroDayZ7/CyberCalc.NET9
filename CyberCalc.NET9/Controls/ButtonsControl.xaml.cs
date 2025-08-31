using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_CALC_NET_9.Controls
{
    public partial class ButtonsControl : UserControl
    {
        public ButtonsControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty OperatorCommandProperty =
            DependencyProperty.Register(
                "OperatorCommand",
                typeof(ICommand),
                typeof(ButtonsControl),
                new PropertyMetadata(null));

        public ICommand OperatorCommand
        {
            get { return (ICommand)GetValue(OperatorCommandProperty); }
            set { SetValue(OperatorCommandProperty, value); }
        }

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register(
                "ClearCommand",
                typeof(ICommand),
                typeof(ButtonsControl),
                new PropertyMetadata(null));

        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        public static readonly DependencyProperty ClearEntryCommandProperty =
            DependencyProperty.Register(
                "ClearEntryCommand",
                typeof(ICommand),
                typeof(ButtonsControl),
                new PropertyMetadata(null));

        public ICommand ClearEntryCommand
        {
            get { return (ICommand)GetValue(ClearEntryCommandProperty); }
            set { SetValue(ClearEntryCommandProperty, value); }
        }

        public static readonly DependencyProperty NumberCommandProperty =
         DependencyProperty.Register(
             "NumberCommand",
             typeof(ICommand),
             typeof(ButtonsControl),
             new PropertyMetadata(null));

        public ICommand NumberCommand
        {
            get { return (ICommand)GetValue(NumberCommandProperty); }
            set { SetValue(NumberCommandProperty, value); }
        }
        public static readonly DependencyProperty FunctionCommandProperty =
            DependencyProperty.Register(
                "FunctionCommand",
                typeof(ICommand),
                typeof(ButtonsControl),
                new PropertyMetadata(null));

        public ICommand FunctionCommand
        {
            get { return (ICommand)GetValue(FunctionCommandProperty); }
            set { SetValue(FunctionCommandProperty, value); }
        }

        public static readonly DependencyProperty CalculateCommandProperty =
            DependencyProperty.Register(
                "CalculateCommand",
                typeof(ICommand),
                typeof(ButtonsControl),
                new PropertyMetadata(null));

        public ICommand CalculateCommand
        {
            get { return (ICommand)GetValue(CalculateCommandProperty); }
            set { SetValue(CalculateCommandProperty, value); }
        }

    }
}
