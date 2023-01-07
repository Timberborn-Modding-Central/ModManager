using ModManager.AddonEnableSystem;
using ModManager.AddonInstallerSystem;
using ModManager.ExtractorSystem;
using ModManager.ManifestLocationFinderSystem;
using ModManager.StartupSystem;

namespace ModManager.ModSystem
{
    public class ModRegisterer : Singleton<ModRegisterer>, ILoadable
    {
        public const string RegistryId = "Mod";

        public void Load(ModManagerStartupOptions startupOptions)
        {
            AddonInstallerRegistry.Instance.Add(RegistryId, new ModInstaller());
            AddonEnablerRegistry.Instance.Add(RegistryId, new ModEnabler());
            ManifestLocationFinderRegistry.Instance.Add(RegistryId, new ModManifestFinder(startupOptions.Logger));
            AddonExtractorRegistry.Instance.Add(RegistryId, new ModExtractor());
        }
    }
}