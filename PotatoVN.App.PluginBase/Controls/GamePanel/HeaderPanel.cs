using GalgameManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.ComponentModel;
using PotatoVN.App.PluginBase.Helper;

namespace PotatoVN.App.PluginBase.Controls.GamePanel;

public interface IHeaderPanelActions
{
    void OnDeveloperClicked(Galgame game, string developerName);
    void OnTotalPlayTimeClicked(Galgame game);
}

public class HeaderPanel : UserControl
{
    private readonly Galgame _game;
    private readonly StackPanel _developerList;
    private readonly HyperlinkButton _totalPlayTimeButton;
    private bool _timeAsHour = true;

    public IHeaderPanelActions? Actions { get; init; }
    
    public HeaderPanel(Galgame game, IHeaderPanelActions? actions = null)
    {
        DataContext = game;
        _game = game;
        Actions = actions;

        _developerList = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5,
        };

        _totalPlayTimeButton = new HyperlinkButton
        {
            Padding = new(0, 0, 0, 0),
            Content = FormatTotalPlayTime(_game.TotalPlayTime),
        };
        _totalPlayTimeButton.RightTapped += (_, _) =>
        {
            _timeAsHour = !_timeAsHour;
            UpdateTotalPlayTime();
        };
        _totalPlayTimeButton.Click += (_, _) => Actions?.OnTotalPlayTimeClicked(_game);

        _game.Developer.OnValueChanged += _ => UpdateDevelopers();
        _game.PropertyChanged += GameOnPropertyChanged;
        UpdateDevelopers();
        UpdateTotalPlayTime();

        var main = new Grid();
        main.ColumnDefinitions.Add(new ColumnDefinition { Width = new(1, GridUnitType.Auto) });
        main.ColumnDefinitions.Add(new ColumnDefinition { Width = new(1, GridUnitType.Star) });

        main.Children.Add(new Grid
        {
            Height = 250, Margin = Thickness.SmallRightMargin, Padding = Thickness.XSmallLeftTopRightBottomMargin,
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
            Children = { new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                MaxWidth = 250, MaxHeight = 250,
            }.WithBinding(Image.SourceProperty, $"{nameof(game.ImagePath)}.Value") },
        }.InColumn(0));
        
        main.Children.Add(new Grid
        {
            RowDefinitions = { 
                new RowDefinition { Height = new(1, GridUnitType.Auto) },
                new RowDefinition { Height = new(1, GridUnitType.Star) },
            },
            Children =
            {
                new TextBlock
                {
                    Margin = Thickness.XXSmallTopMargin,
                }.WithThemeStyle("TitleTextBlockStyle")
                 .WithBinding(TextBlock.TextProperty, $"{nameof(game.Name)}.Value"),

                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 80,
                    Children =
                    {
                        new StackPanel
                        {
                            Children =
                            {
                                new StackPanel
                                {
                                    Margin = Thickness.SmallTopMargin,
                                    Children =
                                    {
                                        new TextBlock().WithThemeStyle("BodyStrongTextBlockStyle").WithUidText("GalgamePage_Developers", "开发商"),
                                        _developerList,
                                    }
                                },
                                new StackPanel
                                {
                                    Margin = Thickness.SmallTopMargin,
                                    Children =
                                    {
                                        new TextBlock().WithThemeStyle("BodyStrongTextBlockStyle").WithUidText("GalgamePage_LastPlayTime", "上次游玩"),
                                        new TextBlock().WithThemeStyle("BodyTextBlockStyle").WithBinding(TextBlock.TextProperty,
                                            new Binding
                                            {
                                                Path = new PropertyPath(nameof(game.LastPlayTime)),
                                                Mode = BindingMode.OneWay,
                                                Converter = new DateTimeToStringConverter(),
                                            }),
                                    }
                                },
                                new StackPanel
                                {
                                    Margin = Thickness.SmallTopMargin,
                                    Children =
                                    {
                                        new TextBlock().WithThemeStyle("BodyStrongTextBlockStyle").WithUidText("GalgamePage_TotalPlayTime", "总时长"),
                                        _totalPlayTimeButton,
                                    }
                                },
                            }
                        },
                        new StackPanel
                        {
                            Children =
                            {
                                new StackPanel
                                {
                                    Margin = Thickness.SmallTopMargin,
                                    Children =
                                    {
                                        new TextBlock().WithThemeStyle("BodyStrongTextBlockStyle").WithUidText("GalgamePage_SavePosition", "存档位置"),
                                        new TextBlock().WithThemeStyle("BodyTextBlockStyle")
                                            .WithBinding(TextBlock.TextProperty, nameof(game.SavePosition)),
                                    }
                                },
                                new StackPanel
                                {
                                    Margin = Thickness.SmallTopMargin,
                                    Children =
                                    {
                                        new TextBlock().WithThemeStyle("BodyStrongTextBlockStyle").WithUidText("GalgamePage_ExpectedPlayTime", "预计时长"),
                                        new TextBlock().WithThemeStyle("BodyTextBlockStyle")
                                            .WithBinding(TextBlock.TextProperty, $"{nameof(game.ExpectedPlayTime)}.Value"),
                                    }
                                },
                                new StackPanel
                                {
                                    Margin = Thickness.SmallTopMargin,
                                    Children =
                                    {
                                        new TextBlock().WithThemeStyle("BodyStrongTextBlockStyle").WithUidText("GalgamePage_ReleaseDate", "发布日期"),
                                        new TextBlock().WithThemeStyle("BodyTextBlockStyle").WithBinding(TextBlock.TextProperty,
                                            new Binding
                                            {
                                                Path = new PropertyPath($"{nameof(game.ReleaseDate)}.Value"),
                                                Mode = BindingMode.OneWay,
                                                Converter = new DateTimeToStringConverter(),
                                            }),
                                    }
                                },
                            }
                        },
                        new TextBlock
                        {
                            Margin = new(0, 30, 0, 0),
                        }.WithThemeStyle("DisplayTextBlockStyle")
                         .WithBinding(TextBlock.TextProperty, new Binding
                         {
                             Path = new PropertyPath($"{nameof(game.Rating)}.Value"),
                             Mode = BindingMode.OneWay,
                             Converter = new RatingToStringConverter(),
                         }),
                    }
                }.InRow(1)
            }
        }.InColumn(1));

        Content = main;
    }

    private void GameOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Galgame.TotalPlayTime))
            UpdateTotalPlayTime();
    }

    private void UpdateDevelopers()
    {
        _developerList.Children.Clear();
        var dev = _game.Developer.Value;
        if (string.IsNullOrWhiteSpace(dev)) return;

        foreach (var name in dev.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = name.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                var button = new HyperlinkButton
                {
                    Margin = new(0, 0, 0, 1),
                    Padding = new(0, 0, 0, 0),
                    Content = trimmed,
                };

                button.Click += (_, _) => Actions?.OnDeveloperClicked(_game, trimmed);
                _developerList.Children.Add(button);
            }
        }
    }

    private void UpdateTotalPlayTime() => _totalPlayTimeButton.Content = FormatTotalPlayTime(_game.TotalPlayTime);

    private string FormatTotalPlayTime(int minutes)
    {
        if (minutes < 0) minutes = 0;
        if (_timeAsHour)
            return minutes >= 60 ? $"{minutes / 60}h{minutes % 60}m" : $"{minutes}m";
        return minutes + " min";
    }

    private sealed class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not DateTime dateTime || dateTime == DateTime.MinValue)
                return "-";
            if (dateTime.Year == 1)
                return dateTime.ToString("MM-dd");
            return dateTime.ToString("yyyy-MM-dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string str && DateTime.TryParse(str, out var result))
                return result;
            return DateTime.MinValue;
        }
    }

    private sealed class RatingToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float f)
                return f <= 0 ? string.Empty : f.ToString("0.0");
            if (value is double d)
                return d <= 0 ? string.Empty : d.ToString("0.0");
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => 0f;
    }
}