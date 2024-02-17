using Bindito.Core;
using HarmonyLib;
using ModManager.AddonSystem;
using ModManager.ExtractorSystem;
using ModManager.LoggingSystem;
using ModManagerUI.CrashGuardSystem;
using Timberborn.GameExitSystem;
using Timberborn.MainMenuScene;

namespace ModManagerUI.UiSystem
{
    [HarmonyPatch]
    public class ModManagerConfigurator
    {
        [HarmonyPatch(typeof(MainMenuSceneConfigurator), nameof(MainMenuSceneConfigurator.Configure))]
        [HarmonyPostfix]
        public static void ConfigurePostfix(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<ModManagerRegisterer>().AsSingleton();
            containerDefinition.Bind<IAddonService>().ToInstance(AddonService.Instance);
            containerDefinition.Bind<ModManagerPanel>().AsSingleton();
            containerDefinition.Bind<AddonExtractorService>().ToInstance(AddonExtractorService.Instance);
            containerDefinition.Bind<GoodbyeBoxFactory>().AsSingleton();
            containerDefinition.Bind<ModFullInfoController>().AsSingleton();
            containerDefinition.Bind<InstalledAddonRepository>().ToInstance(InstalledAddonRepository.Instance);
            containerDefinition.Bind<CrashGuardController>().AsSingleton();
            containerDefinition.Bind<CrashScreenBox>().AsSingleton();
            containerDefinition.Bind<UpdateableModRegistry>().AsSingleton();
            containerDefinition.Bind<IModManagerLogger>().To<ModManagerLogger>().AsSingleton();
        }
    }
}