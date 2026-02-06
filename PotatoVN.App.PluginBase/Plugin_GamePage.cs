using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalgameManager.Enums;
using GalgameManager.Models;
using GalgameManager.WinApp.Base.Contracts;
using GalgameManager.WinApp.Base.Contracts.NavigationApi;
using GalgameManager.WinApp.Base.Contracts.NavigationApi.NavigateParameters;
using GalgameManager.WinApp.Base.Contracts.PluginUi;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PotatoVN.App.PluginBase.Controls;
using PotatoVN.App.PluginBase.Controls.GamePanel;
using PotatoVN.App.PluginBase.Models;

namespace PotatoVN.App.PluginBase;

public partial class Plugin : IGalgamePage, IGalgamePageSetting,
    IHeaderPanelActions, ITagPanelActions, ICharacterPanelActions,
    IStaffPanelProvider, IStaffPanelActions
{
    public async Task SettingAsync()
    {
        PageSettingDialog dialog = new();
        await dialog.ShowAsync();
    }

    public async Task<FrameworkElement> CreateUiAsync(Galgame game)
    {
        await Task.CompletedTask;
        ScrollViewer main = new()
        {
            IsTabStop = true,
            Content = new StackPanel
            {
                Children =
                {
                    new HeaderPanel(game, actions: this),
                    new DescriptionPanel(game),
                    new TagPanel(game, actions: this),
                    new CharacterPanel(game, actions: this),
                    new StaffPanel(game, provider: this, actions: this),
                }
            }
        };

        return main;
    }

    #region IHeaderPanelActions

    public void OnDeveloperClicked(Galgame game, string developerName)
    {
        developerName = developerName.Trim().ToLower();
        List<Galgame> targetGames = HostApi.GetAllGames().Where(g => (g.Developer.Value ?? string.Empty)
                .ToLower().Contains(developerName)).ToList();
        HostApi.AddFilter(new GameListFilter(developerName, targetGames));
        HostApi.NavigateTo(PageEnum.GameListPage);
    }

    public void OnTotalPlayTimeClicked(Galgame game)
    {
        HostApi.NavigateTo(PageEnum.PlayedTimePage, game);
    }

    #endregion

    #region ITagPanelActions

    public void OnTagClicked(Galgame game, string tag)
    {
        // 导航到按标签筛选的库页面
        HostApi.NavigateTo(PageEnum.LibraryPage);
    }

    #endregion

    #region ICharacterPanelActions

    public void OnCharacterClicked(Galgame game, GalgameCharacter character)
    {
        HostApi.NavigateTo(PageEnum.GalgameCharacterPage,
            new GalgameCharacterPageNavParameter { GalgameCharacter = character });
    }

    public void OnAddCharacterRequested(Galgame game, GalgameCharacter? contextCharacter)
    {
        // 导航到角色页面，宿主会处理添加逻辑
        HostApi.NavigateTo(PageEnum.GalgameCharacterPage,
            new GalgameCharacterPageNavParameter
            {
                GalgameCharacter = contextCharacter ?? new GalgameCharacter()
            });
    }

    public void OnDeleteCharacterRequested(Galgame game, GalgameCharacter? contextCharacter)
    {
        if (contextCharacter is null) return;
        game.Characters.Remove(contextCharacter);
    }

    #endregion

    #region IStaffPanelProvider / IStaffPanelActions

    public IReadOnlyList<StaffItem> GetStaffs(Galgame game)
    {
        var staffs = HostApi.GetStaffs(game);
        return staffs.Select(s =>
        {
            var relation = s.GetRelation(game);
            var relationStr = relation is not null && relation.Count > 0
                ? string.Join(", ", relation.Select(CareerToString))
                : string.Empty;
            return new StaffItem(s, relationStr);
        }).ToList();
    }

    public void OnStaffClicked(Galgame game, Staff staff)
    {
        HostApi.NavigateTo(PageEnum.StaffPage, new StaffPageNavParameter { Staff = staff });
    }

    private static string CareerToString(Career career) => career switch
    {
        Career.Writer => "シナリオ",
        Career.Painter => "原画",
        Career.Musician => "音楽",
        Career.Seiyu => "声優",
        Career.Producer => "プロデューサー",
        Career.Programmer => "プログラマー",
        _ => career.ToString(),
    };

    #endregion
}