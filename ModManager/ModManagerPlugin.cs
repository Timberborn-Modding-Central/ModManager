using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using ModManager.ModIoSystem;
using UnityEngine.Networking;

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



            var test = ModIo.Instance.Client.Games[3659].Get();
            test.ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            {
                Log.LogError(test.IsCompleted);
                Log.LogError(test.IsFaulted);
                Log.LogError(test.Result.Name);
            });

        }

        private async void Test()
        {
            await ModIo.Instance.Client.Download(2448112, 2448112, new FileInfo("Test.zip"));
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
