using Modio.Models;
using ModManager.ModSystem;

namespace ModManager.InstallerSystem
{
    public interface IModInstaller
    {
        public bool Install(Mod mod, File file);

        public bool Uninstall(Manifest manifest);

        public bool ChangeVersion(Mod mod, File file);
    }
}