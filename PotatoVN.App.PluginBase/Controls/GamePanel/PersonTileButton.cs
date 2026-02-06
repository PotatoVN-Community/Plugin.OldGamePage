using System;
using GalgameManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using PotatoVN.App.PluginBase.Helper;

namespace PotatoVN.App.PluginBase.Controls.GamePanel;

public sealed class PersonTileButton : Button
{
    private readonly ImageBrush _imageBrush;
    private readonly TextBlock _titleText;
    private readonly TextBlock _subtitleText;

    public PersonTileButton()
    {
        this.WithThemeStyle("TransparentButtonWithHover");

        Height = 130;

        _imageBrush = new ImageBrush
        {
            Stretch = Stretch.UniformToFill,
        };

        var imageRect = new Rectangle
        {
            Height = 75,
            Width = 75,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Fill = _imageBrush,
        };

        _titleText = new TextBlock
        {
            MaxHeight = 40,
            Margin = Thickness.XXSmallTopMargin,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
        }.WithThemeStyle("BodyTextStyle");

        _subtitleText = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Microsoft.UI.Xaml.Thickness(0, -1, 0, 0),
            TextWrapping = TextWrapping.Wrap,
        }.WithThemeStyle("DescriptionTextStyle");

        var stack = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                imageRect,
                _titleText,
                _subtitleText,
            }
        };

        Content = new Grid
        {
            Padding = Thickness.XSmallLeftTopRightBottomMargin,
            Children = { stack }
        };

        SetSubtitle(null);
    }

    public void SetFromCharacter(GalgameCharacter character)
    {
        if (character is null) throw new ArgumentNullException(nameof(character));
        SetTitle(character.Name);
        SetImage(character.PreviewImagePath, fallbackUri: new Uri(Galgame.DefaultCharacterImagePath));
        SetSubtitle(null);
    }

    public void SetFromStaffItem(StaffItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        SetTitle(item.Staff.Name ?? "Unknown");
        SetImage(item.Staff.ImagePath, fallbackUri: DefaultStaffFallbackUri);
        SetSubtitle(item.Relation);
    }

    private static readonly Uri DefaultStaffFallbackUri = new("ms-appx:///Assets/Pictures/Akkarin.webp");

    private void SetTitle(string? title) => _titleText.Text = title ?? string.Empty;

    private void SetSubtitle(string? subtitle)
    {
        var hasSubtitle = !string.IsNullOrWhiteSpace(subtitle);
        _subtitleText.Text = hasSubtitle ? subtitle! : string.Empty;
        _subtitleText.Visibility = hasSubtitle ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SetImage(string? path, Uri? fallbackUri)
    {
        var source = TryCreateImageSource(path);
        if (source is null && fallbackUri is not null)
            source = TryCreateImageSource(fallbackUri.ToString());
        _imageBrush.ImageSource = source;
    }

    private static ImageSource? TryCreateImageSource(string? uri)
    {
        if (string.IsNullOrWhiteSpace(uri)) return null;
        try
        {
            return new BitmapImage(new Uri(uri));
        }
        catch
        {
            return null;
        }
    }
}
