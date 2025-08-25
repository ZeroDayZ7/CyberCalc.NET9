using System.Windows;

namespace WPF_CALC_NET_9.Helpers;

public static class ThemeManager
{
    public static void ApplyTheme(string themeName)
    {
        try
        {
            var appResources = Application.Current.Resources;

            // Usuń poprzedni motyw (ale nie CommonStyles ani AppVersion)
            for (int i = appResources.MergedDictionaries.Count - 1; i >= 0; i--)
            {
                var dict = appResources.MergedDictionaries[i];
                if (dict.Source != null && dict.Source.OriginalString.EndsWith(".xaml") &&
                    !dict.Source.OriginalString.EndsWith("CommonStyles.xaml"))
                {
                    appResources.MergedDictionaries.RemoveAt(i);
                }
            }

            // Dodaj nowy motyw
            var themeDict = new ResourceDictionary { Source = new Uri($"/Styles/{themeName}.xaml", UriKind.Relative) };
            appResources.MergedDictionaries.Add(themeDict);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading theme {themeName}: {ex.Message}", "Theme Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}