using Microsoft.UI.Xaml;

namespace PotatoVN.App.PluginBase.Helper;

public static class ThemeStyleExtensions
{
    public static T WithThemeStyle<T>(this T element, string styleKey) where T : FrameworkElement
    {
        void Apply()
        {
            if (Application.Current.Resources.TryGetValue(styleKey, out var obj) && obj is Style s)
                element.Style = s;
        }

        Apply();
        element.ActualThemeChanged += (_, _) => Apply();
        return element;
    }
}