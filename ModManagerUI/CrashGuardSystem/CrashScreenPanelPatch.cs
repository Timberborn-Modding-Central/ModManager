using System.Linq;
using HarmonyLib;
using Timberborn.CoreUI;
using Timberborn.ErrorReportingUI;
using UnityEngine.UIElements;

namespace ModManagerUI.CrashGuardSystem
{
    [HarmonyPatch]
    public class CrashScreenPanelPatch
    {
        private static CrashScreenPanel? _instance;
        private static bool _runCrashScreen;
        
        [HarmonyPatch(typeof(CrashScreenPanel), "Awake")]
        [HarmonyPrefix]
        public static bool CrashScreenPanelPrefix(CrashScreenPanel __instance, UIDocument ____uiDocument)
        {
            _instance ??= __instance;
            ____uiDocument.rootVisualElement.Children().First().ToggleDisplayStyle(_runCrashScreen);
            return _runCrashScreen;
        }

        public static void ShowCrashScreenPanel()
        {
            if (_instance == null)
                return;
            _runCrashScreen = true;
            _instance.Awake();
            _runCrashScreen = false;
        }
    }
}