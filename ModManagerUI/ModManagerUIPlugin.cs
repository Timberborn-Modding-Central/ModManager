using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModManager.ManifestValidatorSystem;
using ModManager.StartupSystem;
using ModManagerUI.LocalizationSystem;
using System.IO;
using System.Reflection;

namespace ModManagerUI
{
    [BepInPlugin("com.modmanagerui", "Mod Manager UI", "MODMANAGER_VERSION")]
    public class ModManagerUIPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        public void Awake()
        {
            Log = Logger;

            foreach(var file in Directory.GetFiles(Path.Combine(Paths.PluginPath, "ModManager", "libs")))
            {
                Assembly.LoadFile(file);
            }

            ModManagerStartup.Run("MOD_IO_APIKEY", (options) =>
            {
                options.GameId = 3659;
                options.GamePath = BepInEx.Paths.GameRootPath;
                options.IsGameRunning = true;
                options.ModInstallationPath = Path.Combine(Paths.PluginPath);
                options.ModIoGameUrl = "https://mod.io/g/timberborn";
                options.ModManagerPath = Path.Combine(Paths.PluginPath, "ModManager");
            });

            ManifestValidatorService.Instance.ValidateManifests();

            var harmony = new Harmony("com.modmanagerui");
            harmony.PatchAll();
            LocalizationPatcher.Patch(harmony);
        }
    }
}
