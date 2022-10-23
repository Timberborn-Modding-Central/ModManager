using System.IO;
using BepInEx;
using BepInEx.Logging;
using Modio.Models;
using ModManager.ModIoSystem;
using ModManager.ModSystem;
using File = Modio.Models.File;

namespace ModManager
{
    [BepInPlugin("com.modmanager", "Mod Manager", "0.1.0")]
    public class ModManagerPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Paths.LoadPaths();
            LoadDependencies();

            var list = ModIo.Instance.Client.Games[3659].Mods.Search().ToList();

            var service = new ModService();

            list.ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            {
                foreach (Mod? file in list.Result)
                {
                    service.Subscribe(file);
                }
            });

            Log.LogFatal(Paths.ModManager.Data);



            // var test = ModIo.Instance.Client.Games[3659].Mods[2267782].Get();
            // test.ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            // {
            //     Log.LogError(test.IsCompleted);
            //     Log.LogError(test.IsFaulted);
            //     Log.LogError(test.Result);
            // });
        }

        private async void Test()
        {
            await ModIo.Instance.Client.Download(3659, 2410139, new FileInfo("Test.zip"));
        }

        private void LoadDependencies()
        {
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Numerics.Vectors.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Buffers.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Text.Json.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "System.Text.Encodings.Web.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "Microsoft.Bcl.AsyncInterfaces.dll"));
            // Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "Modio.dll"));
        }
    }
}