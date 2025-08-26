using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_CALC_NET_9.Controls
{
    public partial class HeaderButton : UserControl
    {
        public HeaderButton()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(HeaderButton), new PropertyMetadata("Button"));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(HeaderButton), new PropertyMetadata(null));
    }
}
