using System.Collections.Specialized;
using GalgameManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using PotatoVN.App.PluginBase.Helper;

namespace PotatoVN.App.PluginBase.Controls.GamePanel;

public interface ICharacterPanelActions
{
    void OnCharacterClicked(Galgame game, GalgameCharacter character);
    void OnAddCharacterRequested(Galgame game, GalgameCharacter? contextCharacter);
    void OnDeleteCharacterRequested(Galgame game, GalgameCharacter? contextCharacter);
}

public sealed class CharacterPanel : GamePanelBase
{
    private readonly Galgame _game;
    private readonly ItemsRepeater _repeater;
    private GalgameCharacter? _currentContextCharacter;
    private readonly MenuFlyout _flyout;
    private readonly NotifyCollectionChangedEventHandler _charactersChangedHandler;

    public ICharacterPanelActions? Actions { get; init; }

    public CharacterPanel(Galgame game, ICharacterPanelActions? actions = null)
    {
        Game = game;
        _game = game;
        Actions = actions;

        var title = new TextBlock
        {
            Margin = Thickness.SmallTopBottomMargin,
        }.WithThemeStyle("SubtitleTextBlockStyle")
         .WithUidText("GalgamePage_Characters", "角色");

        _repeater = new ItemsRepeater
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new CharacterButtonElementFactory(this),
            ItemsSource = _game.Characters,
            Layout = new UniformGridLayout
            {
                ItemsStretch = UniformGridLayoutItemsStretch.Fill,
                MinItemWidth = 100,
                MinRowSpacing = 10,
            },
        };

        _flyout = new MenuFlyout();
        var addItem = new MenuFlyoutItem { Icon = new SymbolIcon(Symbol.Add) };
        addItem.WithUidText("GameCharacterPanel_AddCharacter", "添加角色");
        addItem.Click += (_, _) => Actions?.OnAddCharacterRequested(_game, _currentContextCharacter);
        _flyout.Items.Add(addItem);
        _flyout.Items.Add(new MenuFlyoutSeparator());
        var delItem = new MenuFlyoutItem { Icon = new SymbolIcon(Symbol.Delete) };
        delItem.WithUidText("GameCharacterPanel_DeleteCharacter", "删除角色");
        delItem.Click += (_, _) => Actions?.OnDeleteCharacterRequested(_game, _currentContextCharacter);
        _flyout.Items.Add(delItem);

        Content = new StackPanel
        {
            Children = { title, _repeater }
        };

        _charactersChangedHandler = (_, _) => UpdateVisibility();
        _game.Characters.CollectionChanged += _charactersChangedHandler;
        UpdateVisibility();
    }

    private void UpdateVisibility() => Visibility = _game.Characters.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

    private void OnCharacterButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not GalgameCharacter character) return;
        Actions?.OnCharacterClicked(_game, character);
    }

    private void OnCharacterButtonRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not Button button) return;
        _currentContextCharacter = button.DataContext as GalgameCharacter;
        _flyout.ShowAt(button, e.GetPosition(button));
    }

    private PersonTileButton BuildCharacterButton()
    {
        var button = new PersonTileButton();
        FlyoutBase.SetAttachedFlyout(button, _flyout);
        button.Click += OnCharacterButtonClick;
        button.RightTapped += OnCharacterButtonRightTapped;
        return button;
    }

    private sealed class CharacterButtonElementFactory : IElementFactory
    {
        private readonly CharacterPanel _owner;
        private readonly System.Collections.Generic.Stack<PersonTileButton> _pool = new();

        public CharacterButtonElementFactory(CharacterPanel owner)
        {
            _owner = owner;
        }

        public UIElement GetElement(ElementFactoryGetArgs args)
        {
            var button = _pool.Count > 0 ? _pool.Pop() : _owner.BuildCharacterButton();
            if (args.Data is GalgameCharacter character)
            {
                button.DataContext = character;
                button.SetFromCharacter(character);
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
