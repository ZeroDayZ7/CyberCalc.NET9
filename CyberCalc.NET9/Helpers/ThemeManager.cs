using System.Diagnostics;
using System.Windows;
using WPF_CALC_NET_9.Properties;

namespace WPF_CALC_NET_9.Helpers;

public static class ThemeManager
{
    public static void ApplyTheme(string themeName)
    {
        try
        {
            var appResources = Application.Current.Resources;

            for (int i = appResources.MergedDictionaries.Count - 1; i >= 0; i--)
            {
                var dict = appResources.MergedDictionaries[i];
                if (dict.Source != null &&
                    dict.Source.OriginalString.EndsWith(".xaml") &&
                    !dict.Source.OriginalString.EndsWith("CommonStyles.xaml"))
                {
                    appResources.MergedDictionaries.RemoveAt(i);
                }
            }

            var themePath = $"/Styles/{themeName}.xaml";
            Debug.WriteLine($"Loading theme: {themePath}");
            var themeDict = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };
            appResources.MergedDictionaries.Add(themeDict);

            Settings.Default.ThemeName = themeName;
            Settings.Default.Save();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading theme {themeName}: {ex.Message}", "Theme Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
