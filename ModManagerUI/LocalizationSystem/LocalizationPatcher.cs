using HarmonyLib;
using MonoMod.Utils;
using System;
using System.Collections.Generic;

namespace ModManagerUI.LocalizationSystem
{
    public class LocalizationPatcher
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(AccessTools.TypeByName("Timberborn.Localization.LocalizationRepository")
                                     .GetMethod("GetLocalization"),
                          postfix: new HarmonyMethod(AccessTools.Method(typeof(LocalizationPatcher), 
                                                                        nameof(GetLocalizationPatch))));
        }

        public static void GetLocalizationPatch(string localizationKey, ref IDictionary<string, string> __result)
        {
            IDictionary<string, string> localization = LocalizationFetcher.GetLocalization(localizationKey);
            try
            {
                __result.AddRange(localization);
                ModManagerUIPlugin.Log.LogInfo($"Loaded {localization.Count} custom labels");
            }
            catch (Exception e)
            {
                ModManagerUIPlugin.Log.LogError(e.ToString());
                throw;
            }
        }
    }
}
