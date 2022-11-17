using System;
using System.IO;
using System.Linq;
using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManager.PersistenceSystem;

namespace ModManager.ModSystem
{
    public class ModInstaller : IAddonInstaller
    {
        private readonly PersistenceService _persistenceService;

        private readonly InstalledAddonRepository _installedAddonRepository;

        private readonly ExtractorService _extractor;

        public ModInstaller()
        {
            _persistenceService = PersistenceService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
            _extractor = ExtractorService.Instance;
        }

        public bool Install(Mod mod, string zipLocation)
        {
            if (!mod.Tags.Any(x => x.Name == "Mod"))
            {
                return false;
            }
            string installLocation = _extractor.ExtractMod(zipLocation, mod);
            var manifest = new Manifest(mod, mod.Modfile, installLocation);
            string modManifestPath = Path.Combine(installLocation, Manifest.FileName);
            _persistenceService.SaveObject(manifest, modManifestPath);
            _installedAddonRepository.Add(manifest);

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            _installedAddonRepository.Remove(manifest.ModId);

            return true;
        }

        public bool ChangeVersion(Mod mod, Modio.Models.File file, string zipLocation)
        {
            throw new NotImplementedException();
        }
    }
}