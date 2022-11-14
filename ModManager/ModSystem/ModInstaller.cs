using System;
using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;
using ModManager.PersistenceSystem;

namespace ModManager.ModSystem
{
    public class ModInstaller : IAddonInstaller
    {
        private readonly PersistenceService _persistenceService;

        private readonly InstalledAddonRepository _installedAddonRepository;

        public ModInstaller()
        {
            _persistenceService = PersistenceService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
        }

        public bool Install(Mod mod, File file)
        {
            Manifest manifest = new Manifest(mod, file, "");

            _persistenceService.SaveObject(manifest, Paths.Mods + "/manifest.json");

            _installedAddonRepository.Add(manifest);

            //TODO: This is just for testing, Hytones implementation needs to be here

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            _installedAddonRepository.Remove(manifest.ModId);

            return true;
        }

        public bool ChangeVersion(Mod mod, File file)
        {
            throw new Exception();
        }
    }
}