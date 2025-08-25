using System;
using System.Windows;

namespace WPF_CALC_NET_9.Helpers;

public static class ThemeManager
{
    public static void ApplyTheme(string themeName)
    {
        try
        {
            var dict = new ResourceDictionary();
            // Do³¹cz CommonStyles.xaml
            dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Styles/CommonStyles.xaml", UriKind.Relative) });
            // Do³¹cz wybrany motyw
            dict.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"/Styles/{themeName}.xaml", UriKind.Relative) });

            // Zast¹p zasoby aplikacji
            Application.Current.Resources = dict;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading theme {themeName}: {ex.Message}", "Theme Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}