using System.Windows;
using System.Windows.Input;
using WPF_CALC_NET_9.ViewModels;

namespace WPF_CALC_NET_9.Views;

public partial class Settings : Window
{
    public Settings()
    {
        InitializeComponent();
        DataContext = Application.Current.MainWindow.DataContext;
    }

    private void ClearHistory_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = (MainViewModel)DataContext;
        viewModel.ClearHistoryCommand.Execute(null);
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}