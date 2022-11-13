using Modio;
using Modio.Models;
using ModManager.AddonSystem;
using ModManager.ModSystem;

namespace ModManager.ModIoSystem
{
    public static class ModClientExtensions
    {
        public static bool IsInstalled(this ModClient modClient)
        {
            return InstalledAddonRepository.Instance.Has(modClient.ModId);
        }

        public static bool IsInstalled(this Mod mod)
        {
            return InstalledAddonRepository.Instance.Has(mod.Id);
        }

        public static bool IsEnabled(this ModClient modClient)
        {
            return InstalledAddonRepository.Instance.TryGet(modClient.ModId, out Manifest manifest) && manifest.Enabled;
        }

        public static bool IsEnabled(this Mod mod)
        {
            return InstalledAddonRepository.Instance.TryGet(mod.Id, out Manifest manifest) && manifest.Enabled;
        }
    }
}