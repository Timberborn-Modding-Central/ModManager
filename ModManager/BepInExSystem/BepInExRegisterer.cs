using ModManager.ExtractorSystem;
using ModManager.MapSystem;
using ModManager.StartupSystem;

namespace ModManager.BepInExSystem
{
    public class BepInExRegisterer : Singleton<BepInExRegisterer>, ILoadable
    {
        public const string RegistryId = "BepInEx";

        public void Load(ModManagerStartupOptions startupOptions)
        {
            AddonExtractorRegistry.Instance.Add(RegistryId, new BepInExExtractor());
        }
    }
}
