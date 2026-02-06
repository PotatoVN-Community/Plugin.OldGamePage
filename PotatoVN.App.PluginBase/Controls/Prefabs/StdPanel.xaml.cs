using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace PotatoVN.App.PluginBase.Controls.Prefabs
{
    public sealed partial class StdPanel : UserControl
    {
        public StdPanel(UIElement child)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(5),
                BorderThickness = new Microsoft.UI.Xaml.Thickness(1),
                Padding = new Microsoft.UI.Xaml.Thickness(15, 10, 50, 10),
                BorderBrush = Application.Current.Resources["CardStrokeColorDefaultBrush"] as Brush,
                Background = Application.Current.Resources["LayerFillColorDefaultBrush"] as Brush,
                Child = child,
            };
            Content = border;
        }
    }

}