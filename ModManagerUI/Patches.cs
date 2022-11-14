using Bindito.Core;
using HarmonyLib;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
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


        [HarmonyPatch(typeof(MainMenuSceneConfigurator), nameof(MainMenuSceneConfigurator.Configure))]
        [HarmonyPostfix]
        public static void ConfigurePostfix(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<IAddonService>().To<AddonService>().AsSingleton();
            containerDefinition.Bind<ModsBox>().AsSingleton();
            containerDefinition.Bind<ExtractorService>().AsSingleton();
        }
    }
}
