using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PotatoVN.App.PluginBase.Helper;

public static class GridExtension
{
    public static T InRow<T>(this T element, int row) where T : FrameworkElement
    {
        Grid.SetRow(element, row);
        return element;
    }

    public static T InColumn<T>(this T element, int column) where T : FrameworkElement
    {
        Grid.SetColumn(element, column);
        return element;
    }
}