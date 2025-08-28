using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_CALC_NET_9.Controls
{
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        public string HeaderText
        {
            get => HeaderTextBlock.Text;
            set => HeaderTextBlock.Text = value;
        }
    }
}
