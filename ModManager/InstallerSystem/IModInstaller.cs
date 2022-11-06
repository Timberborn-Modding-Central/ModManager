using Modio.Models;
using ModManager.ModSystem;

namespace ModManager.InstallerSystem
{
    public interface IModInstaller
    {
        public bool Install(Mod mod, File file, out string? installationPath);

        public bool Uninstall(Mod mod, Manifest manifest);

        public bool ChangeVersion(Mod mod, File file, out string? installationPath);
    }
}