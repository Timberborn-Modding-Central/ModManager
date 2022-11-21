using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModManager.ModIoSystem;
using ModManager.StartupSystem;
using ModManagerUI.LocalizationSystem;
using System;
using System.Collections.Generic;
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
            // Add ModManagerStartup.Run() here

            var harmony = new Harmony("com.modmanagerui");
            harmony.PatchAll();
            LocalizationPatcher.Patch(harmony);
        }
    }
}
