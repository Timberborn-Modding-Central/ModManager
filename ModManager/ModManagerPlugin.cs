using System.IO;
using System.Reflection;
using BepInEx;

namespace ModManager
{
    [BepInPlugin("com.modmanager", "Mod Manager", "0.1.0")]
    public class ModManagerPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Paths.LoadPaths();
            LoadDependencies();
        }

        private void LoadDependencies()
        {
            Assembly.LoadFrom(Path.Combine(Paths.ModManager, "libs", "Modio.dll"));
        }
    }
}
