using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_CALC_NET_9.Controls
{
    public partial class DraggableHeader : UserControl
    {
        public DraggableHeader()
        {
            InitializeComponent();
        }

        public string HeaderText
        {
            get => HeaderTextBlock.Text;
            set => HeaderTextBlock.Text = value;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this)?.DragMove();
        }
    }
}
