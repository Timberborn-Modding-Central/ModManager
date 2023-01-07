using ModManager.AddonEnableSystem;
using ModManager.AddonInstallerSystem;
using ModManager.ExtractorSystem;
using ModManager.ManifestLocationFinderSystem;
using ModManager.ManifestValidatorSystem;
using ModManager.StartupSystem;

namespace ModManager.MapSystem
{
    public class MapRegisterer : Singleton<MapRegisterer>, ILoadable
    {
        public const string RegistryId = "Map";

        public void Load(ModManagerStartupOptions startupOptions)
        {
            AddonInstallerRegistry.Instance.Add(RegistryId, new MapInstaller(startupOptions));
            AddonEnablerRegistry.Instance.Add(RegistryId, new MapEnabler());
            ManifestLocationFinderRegistry.Instance.Add(RegistryId, new MapManifestFinder(startupOptions.Logger));
            AddonExtractorRegistry.Instance.Add(RegistryId, new MapExtractor());
            ManifestValidatorRegistry.Instance.Add(RegistryId, new MapManifestValidator(startupOptions));
        }
    }
}