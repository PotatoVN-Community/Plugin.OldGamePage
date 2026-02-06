using Microsoft.UI.Xaml;

namespace PotatoVN.App.PluginBase.Controls;

public static class Thickness
{
    public static readonly Microsoft.UI.Xaml.Thickness LargeTopMargin = new Microsoft.UI.Xaml.Thickness(0, 36, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness LargeTopBottomMargin = new Microsoft.UI.Xaml.Thickness(0, 36, 0, 36);

    public static readonly Microsoft.UI.Xaml.Thickness MediumTopMargin = new Microsoft.UI.Xaml.Thickness(0, 24, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness MediumTopBottomMargin = new Microsoft.UI.Xaml.Thickness(0, 24, 0, 24);
    public static readonly Microsoft.UI.Xaml.Thickness MediumLeftRightMargin = new Microsoft.UI.Xaml.Thickness(24, 0, 24, 0);
    public static readonly Microsoft.UI.Xaml.Thickness MediumRightMargin = new Microsoft.UI.Xaml.Thickness(0, 0, 24, 0);
    public static readonly Microsoft.UI.Xaml.Thickness MediumBottomMargin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 24);

    public static readonly Microsoft.UI.Xaml.Thickness SmallLeftMargin = new Microsoft.UI.Xaml.Thickness(12, 0, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness SmallLeftRightMargin = new Microsoft.UI.Xaml.Thickness(12, 0, 12, 0);
    public static readonly Microsoft.UI.Xaml.Thickness SmallTopMargin = new Microsoft.UI.Xaml.Thickness(0, 12, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness SmallRightMargin = new Microsoft.UI.Xaml.Thickness(0, 0, 12, 0);
    public static readonly Microsoft.UI.Xaml.Thickness SmallTopBottomMargin = new Microsoft.UI.Xaml.Thickness(0, 12, 0, 12);
    public static readonly Microsoft.UI.Xaml.Thickness SmallBottomMargin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 12);

    public static readonly Microsoft.UI.Xaml.Thickness XSmallLeftMargin = new Microsoft.UI.Xaml.Thickness(8, 0, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness XSmallTopMargin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness XSmallBottomMargin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 8);
    public static readonly Microsoft.UI.Xaml.Thickness XSmallLeftTopRightBottomMargin = new Microsoft.UI.Xaml.Thickness(8, 8, 8, 8);

    public static readonly Microsoft.UI.Xaml.Thickness XXSmallTopMargin = new Microsoft.UI.Xaml.Thickness(0, 4, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness XXSmallLeftTopRightBottomMargin = new Microsoft.UI.Xaml.Thickness(4, 4, 4, 4);

    public static readonly Microsoft.UI.Xaml.Thickness NavigationViewContentGridBorderThickness = new Microsoft.UI.Xaml.Thickness(1, 1, 0, 0);
    public static readonly CornerRadius NavigationViewContentGridCornerRadius = new CornerRadius(8, 0, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness NavigationViewContentMargin = new Microsoft.UI.Xaml.Thickness(0, 48, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness NavigationViewHeaderMargin = new Microsoft.UI.Xaml.Thickness(56, 34, 0, 0);
    public static readonly Microsoft.UI.Xaml.Thickness NavigationViewPageContentMargin = new Microsoft.UI.Xaml.Thickness(56, 24, 56, 0);

    public static readonly Microsoft.UI.Xaml.Thickness MenuBarContentMargin = new Microsoft.UI.Xaml.Thickness(36, 24, 36, 0);

    public const double SettingsPageStackPanelSpacing = 13d;
    public static readonly Microsoft.UI.Xaml.Thickness SettingsPageHyperlinkButtonMargin = new Microsoft.UI.Xaml.Thickness(-12, 0, 0, 0);

    public static readonly Microsoft.UI.Xaml.Thickness PageButtonMargin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 40);

    public static readonly Microsoft.UI.Xaml.Thickness CommandBarMargin = new Microsoft.UI.Xaml.Thickness(0, -67, 0, 0);
}