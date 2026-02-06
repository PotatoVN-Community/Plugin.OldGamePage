using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace PotatoVN.App.PluginBase.Controls.Layout;

public sealed class WrapPanel : Panel
{
    public Orientation Orientation { get; set; } = Orientation.Horizontal;
    public double HorizontalSpacing { get; set; }
    public double VerticalSpacing { get; set; }

    protected override Size MeasureOverride(Size availableSize)
    {
        var availableWidth = Sanitize(availableSize.Width);
        var availableHeight = Sanitize(availableSize.Height);

        var spacingPrimary = Orientation == Orientation.Horizontal ? HorizontalSpacing : VerticalSpacing;
        var spacingSecondary = Orientation == Orientation.Horizontal ? VerticalSpacing : HorizontalSpacing;
        var maxPrimary = Orientation == Orientation.Horizontal ? availableWidth : availableHeight;

        double linePrimaryUsed = 0;
        double lineSecondaryMax = 0;
        double totalSecondary = 0;
        double maxPrimaryUsed = 0;

        foreach (var child in Children)
        {
            child.Measure(new Size(availableWidth, availableHeight));
            var childSize = child.DesiredSize;
            var childPrimary = Orientation == Orientation.Horizontal ? childSize.Width : childSize.Height;
            var childSecondary = Orientation == Orientation.Horizontal ? childSize.Height : childSize.Width;

            var requiredPrimary = linePrimaryUsed > 0 ? linePrimaryUsed + spacingPrimary + childPrimary : childPrimary;
            if (requiredPrimary > maxPrimary && linePrimaryUsed > 0)
            {
                maxPrimaryUsed = Math.Max(maxPrimaryUsed, linePrimaryUsed);
                totalSecondary += (totalSecondary > 0 ? spacingSecondary : 0) + lineSecondaryMax;
                linePrimaryUsed = childPrimary;
                lineSecondaryMax = childSecondary;
            }
            else
            {
                linePrimaryUsed = requiredPrimary;
                lineSecondaryMax = Math.Max(lineSecondaryMax, childSecondary);
            }
        }

        if (Children.Count > 0)
        {
            maxPrimaryUsed = Math.Max(maxPrimaryUsed, linePrimaryUsed);
            totalSecondary += (totalSecondary > 0 ? spacingSecondary : 0) + lineSecondaryMax;
        }

        return Orientation == Orientation.Horizontal
            ? new Size(maxPrimaryUsed, totalSecondary)
            : new Size(totalSecondary, maxPrimaryUsed);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var maxPrimary = Sanitize(Orientation == Orientation.Horizontal ? finalSize.Width : finalSize.Height);
        var spacingPrimary = Orientation == Orientation.Horizontal ? HorizontalSpacing : VerticalSpacing;
        var spacingSecondary = Orientation == Orientation.Horizontal ? VerticalSpacing : HorizontalSpacing;

        double cursorPrimary = 0;
        double cursorSecondary = 0;
        double lineSecondaryMax = 0;

        foreach (var child in Children)
        {
            var childSize = child.DesiredSize;
            var childPrimary = Orientation == Orientation.Horizontal ? childSize.Width : childSize.Height;
            var childSecondary = Orientation == Orientation.Horizontal ? childSize.Height : childSize.Width;

            // cursorPrimary already includes trailing spacing from previous item
            if (cursorPrimary + childPrimary > maxPrimary && cursorPrimary > 0)
            {
                cursorPrimary = 0;
                cursorSecondary += lineSecondaryMax + spacingSecondary;
                lineSecondaryMax = 0;
            }

            var x = Orientation == Orientation.Horizontal ? cursorPrimary : cursorSecondary;
            var y = Orientation == Orientation.Horizontal ? cursorSecondary : cursorPrimary;
            child.Arrange(new Rect(new Point(x, y), childSize));

            cursorPrimary += childPrimary + spacingPrimary;
            lineSecondaryMax = Math.Max(lineSecondaryMax, childSecondary);
        }

        return finalSize;
    }

    private static double Sanitize(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return double.MaxValue;
        return value;
    }
}
