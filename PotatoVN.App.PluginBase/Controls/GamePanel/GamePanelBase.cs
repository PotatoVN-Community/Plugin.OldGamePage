using GalgameManager.Models;
using Microsoft.UI.Xaml.Controls;

namespace PotatoVN.App.PluginBase.Controls.GamePanel;

public partial class GamePanelBase : UserControl
{
    protected Galgame? Game;
    
    protected GamePanelBase()
    {
        Loaded += (_, _) => Update();
    }

    /// <summary>
    /// Galgame类中并非所有字段都是自带更新提醒的，部分更新由GalgameViewModel手动告知各个panel，
    /// 若本panel使用的字段全是自动更新的，可忽略这个函数。
    /// </summary>
    protected virtual void Update() { }
}
