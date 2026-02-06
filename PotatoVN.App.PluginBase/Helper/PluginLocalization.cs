using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using GalgameManager.Enums;
using GalgameManager.WinApp.Base.Contracts;

namespace PotatoVN.App.PluginBase.Helper;

public static class PluginLocalization
{
    private static readonly object Sync = new();
    private static IReadOnlyDictionary<string, string> _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public static bool IsInitialized { get; private set; }
    public static string LanguageTag { get; private set; } = "";

    public static void Initialize(IPotatoVnApi hostApi)
    {
        if (hostApi is null) throw new ArgumentNullException(nameof(hostApi));
        Initialize(hostApi.Language, hostApi.GetPluginPath());
    }

    public static void Initialize(LanguageEnum language, string pluginPath)
    {
        lock (Sync)
        {
            LanguageTag = ResolveLanguageTag(language);
            _map = LoadDictionary(pluginPath, LanguageTag);
            IsInitialized = true;
        }
    }

    public static string GetStringOr(string key, string fallback)
    {
        if (string.IsNullOrEmpty(key)) return fallback;
        if (_map.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
            return value;
        return fallback;
    }

    public static string GetStringOrFormat(string key, string fallback, params object[] args)
    {
        var format = GetStringOr(key, fallback);
        try
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }
        catch
        {
            return format;
        }
    }

    private static string ResolveLanguageTag(LanguageEnum language)
    {
        return language switch
        {
            LanguageEnum.ChineseSimplified => "zh-CN",
            LanguageEnum.English => "en-US",
            LanguageEnum.Japanese => "ja-JP",
            LanguageEnum.Auto => CultureInfo.CurrentUICulture.Name,
            _ => CultureInfo.CurrentUICulture.Name,
        };
    }

    private static IReadOnlyDictionary<string, string> LoadDictionary(string pluginPath, string languageTag)
    {
        try
        {
            var stringsDir = Path.Combine(pluginPath, "Strings");
            var exactPath = Path.Combine(stringsDir, languageTag + ".json");
            if (File.Exists(exactPath))
                return LoadJson(exactPath);

            // fallback: try neutral language (e.g. zh from zh-CN)
            var neutral = languageTag.Split('-')[0];
            var neutralPath = Path.Combine(stringsDir, neutral + ".json");
            if (File.Exists(neutralPath))
                return LoadJson(neutralPath);

            // fallback order
            var zhCn = Path.Combine(stringsDir, "zh-CN.json");
            if (File.Exists(zhCn))
                return LoadJson(zhCn);
        }
        catch
        {
            // ignore
        }

        return BuiltinFallback;
    }

    private static IReadOnlyDictionary<string, string> LoadJson(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return dict is null
                ? BuiltinFallback
                : new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return BuiltinFallback;
        }
    }

    private static readonly IReadOnlyDictionary<string, string> BuiltinFallback =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["GalgamePage_Developers.Text"] = "开发商",
            ["GalgamePage_LastPlayTime.Text"] = "上次游玩",
            ["GalgamePage_TotalPlayTime.Text"] = "总时长",
            ["GalgamePage_SavePosition.Text"] = "存档位置",
            ["GalgamePage_ExpectedPlayTime.Text"] = "预计时长",
            ["GalgamePage_ReleaseDate.Text"] = "发布日期",
        };
}
