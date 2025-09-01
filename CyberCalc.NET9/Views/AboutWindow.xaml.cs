using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace WPF_CALC_NET_9.Views
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void OpenLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.Tag is string uri)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
                }
                catch
                {
                    MessageBox.Show("Cannot open link: " + uri);
                }
            }
        }


        private void CopyEmail_MouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText("zerodayz7@proton.me");
                MessageBox.Show("Email copied to clipboard!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Copy failed: {ex.Message}");
            }
        }


    }
}
