#if DEBUG
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GalgameManager.WinApp.Base.Contracts;
using HarmonyLib;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace PotatoVN.App.PluginBase;

internal static class HarmonyHotReloadPatch
{
    private const string HarmonyId = "PotatoVN.App.PluginBase.HotReloadF5";

    private static int _applied;
    private static Harmony? _harmony;
    private static IPotatoVnApi? _api;
    private static Window? _window;
    private static FrameworkElement? _root;
    private static int _reloadInProgress;

    public static void Apply(Window window, IPotatoVnApi api)
    {
        if (Interlocked.Exchange(ref _applied, 1) == 1) return;
        _api = api;
        _window = window;

        _harmony = new Harmony(HarmonyId);
        _harmony.UnpatchAll(HarmonyId);
        try
        {
            _harmony.PatchAll(typeof(HarmonyHotReloadPatch).Assembly);
        }
        catch
        {
            // patch 失败不影响后续：仍可走 KeyDown 事件方案
        }

        // 注意：插件初始化可能在后台线程执行；所有 WinUI 对象访问必须切回 UI 线程
        try
        {
            api.InvokeOnMainThread(() =>
            {
                try
                {
                    TryAttachKeyHandler();
                    window.Activated -= WindowOnActivated;
                    window.Activated += WindowOnActivated;
                }
                catch
                {
                    // ignore
                }
            });
        }
        catch
        {
            // ignore
        }
    }

    public static void Unapply()
    {
        void UnapplyOnUiThread()
        {
            try
            {
                if (_root is not null)
                {
                    _root.KeyDown -= RootOnKeyDown;
                    _root = null;
                }

                if (_window is not null)
                {
                    _window.Activated -= WindowOnActivated;
                }
            }
            catch
            {
                // ignore
            }
        }

        try
        {
            var api = _api;
            if (api is not null) api.InvokeOnMainThread(UnapplyOnUiThread);
            else UnapplyOnUiThread();
        }
        catch
        {
            // ignore
        }

        try
        {
            _harmony?.UnpatchAll(HarmonyId);
        }
        catch
        {
            // ignore
        }

        _harmony = null;
        _api = null;
        _window = null;
        Interlocked.Exchange(ref _applied, 0);
    }

    private static void WindowOnActivated(object sender, WindowActivatedEventArgs e) => TryAttachKeyHandler();

    private static void TryAttachKeyHandler()
    {
        try
        {
            if (_window?.Content is not FrameworkElement root) return;

            if (!ReferenceEquals(_root, root))
            {
                if (_root is not null) _root.KeyDown -= RootOnKeyDown;
                _root = root;
                _root.KeyDown -= RootOnKeyDown;
                _root.KeyDown += RootOnKeyDown;
            }
        }
        catch
        {
            // ignore
        }
    }

    private static async void RootOnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != Windows.System.VirtualKey.F5) return;

        if (Interlocked.CompareExchange(ref _reloadInProgress, 1, 0) != 0) return;

        e.Handled = true;

        try
        {
            var api = _api;
            if (api is null) return;

            var deleteData = false;
            try
            {
                var state = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
                deleteData = state.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            }
            catch
            {
                // ignore
            }

            api.Info(InfoBarSeverity.Informational,
                msg: deleteData ? "F5(Shift) 已触发：正在热重载插件（含删除数据）..." : "F5 已触发：正在热重载插件...",
                displayTimeMs: 1500);

            await HotReloadCurrentDevPluginAsync(api, deleteData);

            api.Info(InfoBarSeverity.Success, msg: "插件热重载完成", displayTimeMs: 2500);
        }
        catch
        {
            // ignore
        }
        finally
        {
            Interlocked.Exchange(ref _reloadInProgress, 0);
        }
    }

    private static async Task HotReloadCurrentDevPluginAsync(IPotatoVnApi api, bool deleteData)
    {
        object? pluginX = TryGetPluginXFromApiHost(api);
        if (pluginX is null)
        {
            api.Info(InfoBarSeverity.Warning, msg: "热重载失败：无法定位当前插件实例（PluginX）", displayTimeMs: 3000);
            return;
        }

        var pluginXType = pluginX.GetType();
        var isDevMode = AccessTools.Property(pluginXType, "IsDevMode")?.GetValue(pluginX) as bool? ?? false;
        if (!isDevMode)
        {
            api.Info(InfoBarSeverity.Warning, msg: "仅 Dev 模式插件支持 F5 热重载", displayTimeMs: 3000);
            return;
        }

        var pluginPath = AccessTools.Property(pluginXType, "Path")?.GetValue(pluginX) as string;
        if (string.IsNullOrWhiteSpace(pluginPath))
        {
            api.Info(InfoBarSeverity.Warning, msg: "热重载失败：插件路径为空", displayTimeMs: 3000);
            return;
        }

        object? pluginService = TryGetPluginService();
        if (pluginService is null)
        {
            api.Info(InfoBarSeverity.Warning, msg: "热重载失败：无法获取宿主 IPluginService", displayTimeMs: 3000);
            return;
        }

        var serviceType = pluginService.GetType();
        var deleteMethod = AccessTools.Method(serviceType, "DeletePluginAsync");
        var addMethod = AccessTools.Method(serviceType, "AddPluginAsync");
        if (deleteMethod is null || addMethod is null)
        {
            api.Info(InfoBarSeverity.Warning, msg: "热重载失败：宿主插件服务 API 变化（找不到 Delete/Add）", displayTimeMs: 3000);
            return;
        }

        var deleteTaskObj = deleteMethod.Invoke(pluginService, new[] { pluginX, (object)deleteData });
        if (deleteTaskObj is Task deleteTask) await deleteTask;

        var addTaskObj = addMethod.Invoke(pluginService, new object[] { pluginPath, true });
        if (addTaskObj is Task addTask) await addTask;
    }

    private static object? TryGetPluginService()
    {
        var appType = AccessTools.TypeByName("GalgameManager.App");
        if (appType is null) return null;

        var iPluginServiceType = AccessTools.TypeByName("GalgameManager.Contracts.Services.IPluginService");
        if (iPluginServiceType is null) return null;

        var getService = AccessTools.Method(appType, "GetService");
        if (getService is null || !getService.IsGenericMethodDefinition) return null;

        var generic = getService.MakeGenericMethod(iPluginServiceType);
        return generic.Invoke(null, null);
    }

    private static object? TryGetPluginXFromApiHost(IPotatoVnApi api)
    {
        var apiType = api.GetType();

        var field = AccessTools.Field(apiType, "plugin");
        if (field is not null)
            return field.GetValue(api);

        var pluginXField = apiType
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .FirstOrDefault(f => string.Equals(f.FieldType.FullName, "GalgameManager.Models.PluginX", StringComparison.Ordinal));
        return pluginXField?.GetValue(api);
    }

    [HarmonyPatch]
    private static class _HarmonyMarkerPatch
    {
        private static MethodBase? TargetMethod()
        {
            var windowType = AccessTools.TypeByName("GalgameManager.MainWindow");
            return windowType is null ? null : AccessTools.Constructor(windowType);
        }

        private static void Postfix()
        {
            // no-op
        }
    }
}
#endif
