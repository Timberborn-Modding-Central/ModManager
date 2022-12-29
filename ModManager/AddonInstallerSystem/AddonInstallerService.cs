using Modio.Models;
using ModManager.AddonSystem;
using ModManager.ModSystem;

namespace ModManager.AddonInstallerSystem
{
    public class AddonInstallerService : Singleton<AddonInstallerService>
    {
        private readonly AddonInstallerRegistry _addonInstallerRegistry;

        public AddonInstallerService()
        {
            _addonInstallerRegistry = AddonInstallerRegistry.Instance;
        }

        public void Install(Mod mod, string zipLocation)
        {
            foreach (IAddonInstaller installer in _addonInstallerRegistry.GetAddonInstallers())
            {
                if (installer.Install(mod, zipLocation))
                {
                    return;
                }
            }

            throw new AddonInstallerException($"{mod.Name} could not be installed by any installer");
        }

        public void Uninstall(Manifest manifest)
        {
            foreach (IAddonInstaller installer in _addonInstallerRegistry.GetAddonInstallers())
            {
                if (installer.Uninstall(manifest))
                {
                    return;
                }
            }

            throw new AddonInstallerException($"{manifest.ModName} could not be uninstalled by any installer");
        }

        public void ChangeVersion(Mod mod, File file, string zipLocation)
        {
            foreach (IAddonInstaller installer in _addonInstallerRegistry.GetAddonInstallers())
            {
                if (installer.ChangeVersion(mod, file, zipLocation))
                {
                    return;
                }
            }

            throw new AddonInstallerException($"The version of {mod.Name} could not be changed by any installer");
        }
    }
}