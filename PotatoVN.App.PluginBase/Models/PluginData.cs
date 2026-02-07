using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PotatoVN.App.PluginBase.Models;

public partial class PluginData : ObservableRecipient
{
    /// <summary>
    /// 是否显示“简介”面板
    /// </summary>
    [ObservableProperty] private bool _showDescription = true;

    /// <summary>
    /// 是否显示“标签”面板
    /// </summary>
    [ObservableProperty] private bool _showTag = true;

    /// <summary>
    /// 是否显示“角色”面板
    /// </summary>
    [ObservableProperty] private bool _showCharacter = true;

    /// <summary>
    /// 是否显示“Staff”面板
    /// </summary>
    [ObservableProperty] private bool _showStaff = true;

    /// <summary>
    /// 详情页各 Panel 的堆叠顺序（自上而下）。
    /// </summary>
    public List<PanelType> Order { get; set; }= [];

    public void Normalize()
    {
        var required = new[]
        {
            PanelType.Header,
            PanelType.Description,
            PanelType.Tag,
            PanelType.Character,
            PanelType.Staff,
        };

        var current = Order.ToHashSet();
        foreach (var panel in required)
        {
            if (!current.Contains(panel))
                Order.Add(panel);
        }
    }
}
