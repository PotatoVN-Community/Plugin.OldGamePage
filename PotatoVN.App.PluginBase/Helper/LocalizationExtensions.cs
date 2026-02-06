using Microsoft.UI.Xaml.Controls;

namespace PotatoVN.App.PluginBase.Helper;

public static class LocalizationExtensions
{
    public static string GetStringOr(this string resourceKey, string fallback)
    {
        return PluginLocalization.GetStringOr(resourceKey, fallback);
    }

    public static TextBlock WithResourceText(this TextBlock textBlock, string resourceKey, string? fallback = null)
    {
        textBlock.Text = resourceKey.GetStringOr(fallback ?? resourceKey);
        return textBlock;
    }

    public static TextBlock WithUidText(this TextBlock textBlock, string uid, string? fallback = null)
        => textBlock.WithResourceText(uid + ".Text", fallback);

    public static T WithUidContent<T>(this T control, string uid, string? fallback = null)
        where T : ContentControl
    {
        control.Content = (uid + ".Text").GetStringOr(fallback ?? uid);
        return control;
    }

    public static MenuFlyoutItem WithUidText(this MenuFlyoutItem item, string uid, string? fallback = null)
    {
        item.Text = (uid + ".Text").GetStringOr(fallback ?? uid);
        return item;
    }
}
