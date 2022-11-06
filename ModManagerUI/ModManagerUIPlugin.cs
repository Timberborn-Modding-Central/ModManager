using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModManagerUI
{
    [BepInPlugin("com.modmanagerui", "Mod Manager UI", "0.1.0")]
    public class ModManagerUIPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        private void Awake()
        {
            new Harmony("com.modmanagerui").PatchAll();
        }
    }
}
