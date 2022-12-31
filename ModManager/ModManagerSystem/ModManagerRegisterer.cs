using ModManager.ExtractorSystem;
using ModManager.StartupSystem;

namespace ModManager.ModManagerSystem
{
    public class ModManagerRegisterer : Singleton<ModManagerRegisterer>, ILoadable
    {
        public const string RegistryId = "ModManager";

        public void Load(ModManagerStartupOptions startupOptions)
        {
            AddonExtractorRegistry.Instance.Add(RegistryId, new ModManagerExtractor());
        }
    }
}
