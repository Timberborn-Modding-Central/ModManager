using System;
using Modio.Models;
using ModManager.ModSystem;
using ModManager.PersistenceSystem;

namespace ModManager.InstallerSystem.Installers
{
    public class GeneralInstaller : IModInstaller
    {
        private readonly PersistenceService _persistenceService;

        public GeneralInstaller()
        {
            _persistenceService = PersistenceService.Instance;
        }

        public bool Install(Mod mod, File file)
        {
            Manifest manifest = new Manifest(mod, file, "");

            _persistenceService.SaveObject(manifest, Paths.Mods + "/manifest.json");

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            throw new Exception();
        }

        public bool ChangeVersion(Mod mod, File file)
        {
            throw new Exception();
        }
    }
}