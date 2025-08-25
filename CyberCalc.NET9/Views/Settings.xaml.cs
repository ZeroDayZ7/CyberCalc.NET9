using System.IO;
using System.Windows;
using System.Windows.Input;
using WPF_CALC_NET_9.ViewModels;

namespace WPF_CALC_NET_9.Views;

public partial class Settings : Window
{
    public Settings()
    {
        InitializeComponent();
        DataContext = new MainViewModel(); // Ustawienie DataContext na MainViewModel dla ThemeCommand
    }

    private void ClearHistory_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (File.Exists("wyniki.txt"))
            {
                File.WriteAllText("wyniki.txt", string.Empty);
                MessageBox.Show("Historia zosta³a wyczyszczona.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"B³¹d podczas czyszczenia historii: {ex.Message}", "B³¹d", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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