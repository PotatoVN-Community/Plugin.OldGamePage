using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PotatoVN.App.PluginBase.Helper;

public static class UiElementExtension
{
    public static T WithBinding<T>(this T element, DependencyProperty dp, string? path, BindingMode mode = BindingMode.OneWay)
        where T : DependencyObject
    {
        var binding = new Binding
        {
            Path = new PropertyPath(path),
            Mode = mode,
        };

        BindingOperations.SetBinding(element, dp, binding);
        return element;
    }

    public static T WithBinding<T>(this T element, DependencyProperty dp, Binding binding)
        where T : DependencyObject
    {
        BindingOperations.SetBinding(element, dp, binding);
        return element;
    }
}