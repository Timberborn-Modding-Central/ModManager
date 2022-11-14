using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;

namespace ModManager.MapSystem
{
    public class MapInstaller : IAddonInstaller
    {
        public bool Install(Mod mod, File file)
        {
            throw new System.NotImplementedException();
        }

        public bool Uninstall(Manifest manifest)
        {
            throw new System.NotImplementedException();
        }

        public bool ChangeVersion(Mod mod, File file)
        {
            throw new System.NotImplementedException();
        }
    }
}