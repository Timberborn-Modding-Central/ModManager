using Modio.Models;
using ModManager.AddonSystem;

namespace ModManager.AddonInstallerSystem
{
    public interface IAddonInstaller
    {
        public bool Install(Mod mod, File file);

        public bool Uninstall(Manifest manifest);

        public bool ChangeVersion(Mod mod, File file);
    }
}