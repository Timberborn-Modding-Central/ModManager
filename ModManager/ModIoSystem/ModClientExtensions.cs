using Modio;
using Modio.Models;
using ModManager.ModSystem;

namespace ModManager.ModIoSystem
{
    public static class ModClientExtensions
    {
        public static bool IsInstalled(this ModClient modClient)
        {
            return InstalledModRepository.Instance.Has(modClient.ModId);
        }

        public static bool IsInstalled(this Mod mod)
        {
            return InstalledModRepository.Instance.Has(mod.Id);
        }
    }
}