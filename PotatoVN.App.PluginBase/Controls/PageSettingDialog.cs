using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PotatoVN.App.PluginBase.Controls;

public class PageSettingDialog : ContentDialog
{
    public PageSettingDialog()
    {
        XamlRoot = Plugin.HostApi.GetMainWindow()!.Content!.XamlRoot;
        RequestedTheme = RequestedTheme = Plugin.HostApi.GetMainWindow()!.Content is FrameworkElement element
            ? element.RequestedTheme : RequestedTheme;
        Title = "Hello World";
        PrimaryButtonText = "OK";
        DefaultButton = ContentDialogButton.Primary;
    }
}