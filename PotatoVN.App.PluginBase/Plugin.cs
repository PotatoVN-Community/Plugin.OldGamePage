using System;
using System.Threading;
using System.Threading.Tasks;
using GalgameManager.WinApp.Base.Contracts;
using GalgameManager.WinApp.Base.Contracts.PluginUi;
using GalgameManager.WinApp.Base.Models;
using PotatoVN.App.PluginBase.Models;
using PotatoVN.App.PluginBase.Helper;

namespace PotatoVN.App.PluginBase
{
    public partial class Plugin : IPlugin
    {
        public static IPotatoVnApi HostApi { get; private set; } = null!;
        private PluginData _data = new ();
        
        public PluginInfo Info { get; } = new()
        {
            Id = new Guid("57ca448f-675c-4461-bfd2-efb9810d1051"),
            Name = "旧版游戏详情页",
            Description = "提供一个类似于PotatoVN 1.8以前版本的游戏详情页。\n曾经这个功能内嵌在软件本体中，现分拆成本插件提供。",
        };

        public async Task InitializeAsync(IPotatoVnApi hostApi)
        {
            HostApi = hostApi;
            PluginLocalization.Initialize(HostApi);
            XamlResourceLocatorFactory.packagePath = HostApi.GetPluginPath();
            var dataJson = await HostApi.GetDataAsync();
            if (!string.IsNullOrWhiteSpace(dataJson))
            {
                try
                {
                    _data = System.Text.Json.JsonSerializer.Deserialize<PluginData>(dataJson) ?? new PluginData();
                }
                catch
                {
                    _data = new PluginData();
                }
            }

            _data.PropertyChanged += (_, _) => SaveData(); // 当Observable属性变化时自动保存数据，对于普通属性请手动调用SaveData


            var window = HostApi.GetMainWindow();
            if (window == null) return;

#if DEBUG
            HarmonyHotReloadPatch.Apply(window, HostApi);
#endif
        }

        public Task OnUninstallAsync(bool deleteData, Action<TimeSpan> extendWaitHandler, CancellationToken cts)
        {
#if DEBUG
            HarmonyHotReloadPatch.Unapply();
#endif
            return Task.CompletedTask;
        }

        private void SaveData()
        {
            var dataJson = System.Text.Json.JsonSerializer.Serialize(_data);
            _ = HostApi.SaveDataAsync(dataJson);
        }

        protected Guid Id => Info.Id;
    }
}