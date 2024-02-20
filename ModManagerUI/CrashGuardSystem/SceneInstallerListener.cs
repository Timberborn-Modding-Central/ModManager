using HarmonyLib;
using Timberborn.GameScene;
using Timberborn.MapEditorScene;

namespace ModManagerUI.CrashGuardSystem
{
    [HarmonyPatch]
    public class SceneInstallerListener
    {
        [HarmonyPatch(typeof(GameSceneInstaller), "Configure")]
        [HarmonyPatch(typeof(MapEditorSceneInstaller), "Configure")]
        [HarmonyPostfix]
        public static void SceneInstallerPrefix()
        {
            CrashGuardController.Disable();
        }
    }
}