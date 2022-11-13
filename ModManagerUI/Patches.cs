using HarmonyLib;
using System;
using Timberborn.MainMenuScene;
using Timberborn.ModsSystemUI;
using UnityEngine.UIElements;

namespace ModManagerUI
{
    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPatch(typeof(MainMenuPanel), "GetPanel")]
        [HarmonyPostfix]
        public static void MainMenuPanelPostfix(ref VisualElement __result)
        {
            VisualElement root = __result.Query("MainMenuPanel");
            Button button = new Button() { classList = { "menu-button" } };
            button.text = "Mod manager";
            button.clicked += ModsBox.OpenOptionsDelegate;
            root.Insert(7, button);
        }
    }
}
