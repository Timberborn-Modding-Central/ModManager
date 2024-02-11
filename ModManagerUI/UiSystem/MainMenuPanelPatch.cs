using HarmonyLib;
using Timberborn.CoreUI;
using Timberborn.MainMenuScene;
using UnityEngine.UIElements;

namespace ModManagerUI.UiSystem
{
    [HarmonyPatch]
    public class MainMenuPanelPatch
    {
        [HarmonyPatch(typeof(MainMenuPanel), "GetPanel")]
        [HarmonyPostfix]
        public static void MainMenuPanelPostfix(ref VisualElement __result)
        {
            VisualElement root = __result.Query("MainMenuPanel");
            var button = new LocalizableButton
            {
                classList = {"menu-button", "menu-button--stretched" }, 
                text = "Mod manager"
            };
            button.clicked += () => ModManagerPanel.Instance.OpenOptionsDelegate();
            root.Insert(7, button);
        }
    }
}
