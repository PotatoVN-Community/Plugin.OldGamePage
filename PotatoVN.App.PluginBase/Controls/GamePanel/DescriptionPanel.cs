using GalgameManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PotatoVN.App.PluginBase.Helper;

namespace PotatoVN.App.PluginBase.Controls.GamePanel;

public sealed class DescriptionPanel : GamePanelBase
{
    public DescriptionPanel(Galgame game)
    {
        Game = game;
        DataContext = Game;

        var title = new TextBlock
        {
            Margin = Thickness.SmallTopBottomMargin,
        }.WithThemeStyle("SubtitleTextBlockStyle")
         .WithUidText("GalgamePage_Description", "简介");

        var body = new TextBlock
        {
            IsTextSelectionEnabled = true,
        }.WithThemeStyle("BodyTextBlockStyle")
         .WithBinding(TextBlock.TextProperty, $"{nameof(game.Description)}.Value");
        
        Content = new StackPanel
        {
            Children = { title, body }
        };

        game.Description.OnValueChanged += _ => Update();
        Update();
    }

    protected override void Update()
    {
        if (Game is null)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        var text = Game.Description.Value;
        Visibility = string.IsNullOrWhiteSpace(text) ? Visibility.Collapsed : Visibility.Visible;
    }
}
