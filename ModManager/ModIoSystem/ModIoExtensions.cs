using Modio.Models;
using ModManager.AddonSystem;

namespace ModManager.ModIoSystem
{
    public static class ModIoExtensions
    {
        public static bool IsInstalled(this Mod mod)
        {
            return InstalledAddonRepository.Instance.Has(mod.Id);
        }
        
        public static bool IsInstalled(this Dependency dependency)
        {
            return InstalledAddonRepository.Instance.Has(dependency.ModId);
        }

        public static bool IsEnabled(this Mod mod)
        {
            return InstalledAddonRepository.Instance.TryGet(mod.Id, out var manifest) && manifest.Enabled;
        }
        
        public static bool IsModInstalled(this File mod)
        {
            return InstalledAddonRepository.Instance.Has(mod.ModId);
        }
    }
}