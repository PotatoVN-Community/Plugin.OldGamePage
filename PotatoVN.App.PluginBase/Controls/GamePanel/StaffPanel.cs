using System;
using System.Collections.Generic;
using GalgameManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using PotatoVN.App.PluginBase.Helper;

namespace PotatoVN.App.PluginBase.Controls.GamePanel;

public sealed record StaffItem(Staff Staff, string Relation);

public interface IStaffPanelProvider
{
    IReadOnlyList<StaffItem> GetStaffs(Galgame game);
}

public interface IStaffPanelActions
{
    void OnStaffClicked(Galgame game, Staff staff);
}

public sealed class StaffPanel : GamePanelBase
{
    private readonly Galgame _game;
    private readonly ItemsRepeater _repeater;
    private IReadOnlyList<StaffItem> _staffs = Array.Empty<StaffItem>();

    public IStaffPanelProvider? Provider { get; init; }
    public IStaffPanelActions? Actions { get; init; }

    public StaffPanel(Galgame game, IStaffPanelProvider? provider = null, IStaffPanelActions? actions = null)
    {
        Game = game;
        _game = game;
        Provider = provider;
        Actions = actions;

        var title = new TextBlock
        {
            Margin = Thickness.SmallTopBottomMargin,
        }.WithThemeStyle("SubtitleTextBlockStyle")
         .WithUidText("GalgamePage_Staffs", "Staff");

        _repeater = new ItemsRepeater
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new StaffButtonElementFactory(this),
            ItemsSource = _staffs,
            Layout = new UniformGridLayout
            {
                ItemsStretch = UniformGridLayoutItemsStretch.Fill,
                MinItemWidth = 100,
                MinRowSpacing = 10,
            },
        };

        Content = new StackPanel
        {
            Children =
            {
                title,
                _repeater,
            }
        };

        Update();
    }

    protected override void Update()
    {
        if (Provider is null)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        _staffs = Provider.GetStaffs(_game) ?? Array.Empty<StaffItem>();
        if (_staffs.Count == 0)
        {
            Visibility = Visibility.Collapsed;
            _repeater.ItemsSource = Array.Empty<StaffItem>();
            return;
        }

        Visibility = Visibility.Visible;
        _repeater.ItemsSource = _staffs;
    }

    private void OnStaffButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not StaffItem item) return;
        Actions?.OnStaffClicked(_game, item.Staff);
    }

    private PersonTileButton BuildStaffButton()
    {
        var button = new PersonTileButton();
        button.Click += OnStaffButtonClick;
        return button;
    }

    private sealed class StaffButtonElementFactory : IElementFactory
    {
        private readonly StaffPanel _owner;
        private readonly System.Collections.Generic.Stack<PersonTileButton> _pool = new();

        public StaffButtonElementFactory(StaffPanel owner)
        {
            _owner = owner;
        }

        public UIElement GetElement(ElementFactoryGetArgs args)
        {
            var button = _pool.Count > 0 ? _pool.Pop() : _owner.BuildStaffButton();
            if (args.Data is StaffItem item)
            {
                button.DataContext = item;
                button.SetFromStaffItem(item);
            }
            else
            {
                button.DataContext = null;
            }
            return button;
        }

        public void RecycleElement(ElementFactoryRecycleArgs args)
        {
            if (args.Element is not PersonTileButton button) return;
            button.DataContext = null;
            _pool.Push(button);
        }
    }
}
