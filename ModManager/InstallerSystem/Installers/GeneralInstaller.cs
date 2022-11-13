using System;
using Modio.Models;
using ModManager.ModSystem;
using ModManager.PersistenceSystem;

namespace ModManager.InstallerSystem.Installers
{
    public class GeneralInstaller : IModInstaller
    {
        private readonly PersistenceService _persistenceService;

        private readonly InstalledModRepository _installedModRepository;

        public GeneralInstaller()
        {
            _persistenceService = PersistenceService.Instance;
            _installedModRepository = InstalledModRepository.Instance;
        }

        public bool Install(Mod mod, File file)
        {
            Manifest manifest = new Manifest(mod, file, "");

            _persistenceService.SaveObject(manifest, Paths.Mods + "/manifest.json");

            _installedModRepository.Add(manifest);

            //TODO: This is just for testing, Hytones implementation needs to be here

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            _installedModRepository.Remove(manifest.ModId);

            return true;
        }

        public bool ChangeVersion(Mod mod, File file)
        {
            throw new Exception();
        }
    }
}