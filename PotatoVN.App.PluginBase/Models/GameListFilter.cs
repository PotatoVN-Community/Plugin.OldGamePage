using System;
using System.Collections.Generic;
using GalgameManager.Models;
using GalgameManager.WinApp.Base.Models.Filters;

namespace PotatoVN.App.PluginBase.Models;

public class GameListFilter : FilterBase
{
    private readonly HashSet<Guid> _gameIds = [];
    
    public GameListFilter(string name, List<Galgame> games)
    {
        Name = name;
        foreach (var game in games) _gameIds.Add(game.Uuid);
    }
    
    public override bool Apply(Galgame galgame) => _gameIds.Contains(galgame.Uuid);

    public override string Name { get; }

    protected override string SuggestName => Name;
}