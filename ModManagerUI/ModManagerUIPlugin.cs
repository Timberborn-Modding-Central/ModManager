using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModManager.ModIoSystem;
using ModManager.StartupSystem;
using ModManagerUI.LocalizationSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ModManagerUI
{
    [BepInPlugin("com.modmanagerui", "Mod Manager UI", "0.1.0")]
    public class ModManagerUIPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        public void Awake()
        {
            Log = Logger; 
            ModManagerStartup.Run("7f52d134de5cde63fdcf163478e688e3", (options) =>
            {
                options.GameId = 3659;
                options.GamePath = BepInEx.Paths.GameRootPath;
                options.IsGameRunning = true;
                //options.ModInstallationPath = Path.Combine(BepInEx.Paths.GameRootPath, "mods");
                options.ModInstallationPath = Path.Combine(Paths.PluginPath);
                options.ModIoGameUrl = "https://mod.io/g/timberborn";
                //options.ModManagerPath = Path.Combine(options.ModInstallationPath, "ModManager", "plugins");
                options.ModManagerPath = Path.Combine(Paths.PluginPath, "ModManager");
            });


            var harmony = new Harmony("com.modmanagerui");
            harmony.PatchAll();
            LocalizationPatcher.Patch(harmony);
        }
    }
}
