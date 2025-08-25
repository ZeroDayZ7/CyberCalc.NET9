using System.Windows;

namespace WPF_CALC_NET_9.Helpers;

public static class ThemeManager
{
    public static void ApplyTheme(string themeName)
    {
        try
        {
            var dict = new ResourceDictionary();
            dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Styles/CommonStyles.xaml", UriKind.Relative) });
            dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"/Styles/{themeName}.xaml", UriKind.Relative) });

            Application.Current.Resources = dict;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading theme {themeName}: {ex.Message}", "Theme Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}