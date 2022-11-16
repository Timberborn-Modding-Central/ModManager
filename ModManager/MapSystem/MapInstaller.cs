using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;

namespace ModManager.MapSystem
{
    public class MapInstaller : IAddonInstaller
    {
        public bool Install(Mod mod, string zipLocation)
        {
            return false;
            throw new System.NotImplementedException();
        }

        public bool Uninstall(Manifest manifest)
        {
            return false;
            throw new System.NotImplementedException();
        }

        public bool ChangeVersion(Mod mod, File file, string zipLocation)
        {
            return false;
            throw new System.NotImplementedException();
        }
    }
}