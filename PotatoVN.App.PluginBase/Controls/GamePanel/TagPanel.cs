using System;
using System.Collections.Specialized;
using GalgameManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PotatoVN.App.PluginBase.Controls.Layout;
using PotatoVN.App.PluginBase.Helper;

namespace PotatoVN.App.PluginBase.Controls.GamePanel;

public interface ITagPanelActions
{
    void OnTagClicked(Galgame game, string tag);
}

public sealed class TagPanel : GamePanelBase
{
    private readonly Galgame _game;
    private readonly WrapPanel _tagsWrap;
    private INotifyCollectionChanged? _currentTags;

    public ITagPanelActions? Actions { get; init; }

    public TagPanel(Galgame game, ITagPanelActions? actions = null)
    {
        Game = game;
        _game = game;
        Actions = actions;

        var title = new TextBlock
        {
            Margin = Thickness.SmallTopBottomMargin,
        }.WithThemeStyle("SubtitleTextBlockStyle")
         .WithUidText("GalgamePage_Tags", "Tags");

        _tagsWrap = new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalSpacing = 15,
            VerticalSpacing = 10,
        };

        Content = new StackPanel
        {
            Children = { title, _tagsWrap }
        };

        _game.Tags.OnValueChanged += _ => RehookTagsCollection();
        RehookTagsCollection();
        Rebuild();
    }

    private void RehookTagsCollection()
    {
        if (_currentTags is not null)
            _currentTags.CollectionChanged -= TagsOnCollectionChanged;

        _currentTags = _game.Tags.Value;
        if (_currentTags is not null)
            _currentTags.CollectionChanged += TagsOnCollectionChanged;
    }

    private void TagsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Rebuild();

    private void Rebuild()
    {
        _tagsWrap.Children.Clear();
        var tags = _game.Tags.Value;

        if (tags is null || tags.Count == 0)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        Visibility = Visibility.Visible;

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag)) continue;
            var localTag = tag;
            var button = new Button
            {
                CornerRadius = new CornerRadius(10),
                Padding = new(0),
                BorderThickness = new(0),
                Margin = new(0),
                Content = new TextBlock
                {
                    Padding = new(5),
                    Text = localTag,
                }
            };

            button.Click += (_, _) => Actions?.OnTagClicked(_game, localTag);
            _tagsWrap.Children.Add(button);
        }
    }
}
