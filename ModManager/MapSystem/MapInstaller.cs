using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManager.PersistenceSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = Modio.Models.File;

namespace ModManager.MapSystem
{
    public class MapInstaller : IAddonInstaller
    {
        private readonly PersistenceService _persistenceService;

        private readonly InstalledAddonRepository _installedAddonRepository;

        private readonly ExtractorService _extractor;

        public MapInstaller()
        {
            _persistenceService = PersistenceService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
            _extractor = ExtractorService.Instance;
        }

        public bool Install(Mod mod, string zipLocation)
        {
            if(!mod.Tags.Any(x => x.Name == "Map"))
            {
                return false;
            }
            string installLocation = _extractor.ExtractMap(zipLocation, mod);

            // TODO: manifest handling is probably wrong atm
            string manifestPath = Path.Combine(installLocation, Manifest.FileName);
            List<MapManifest> manifests = new();
            if (System.IO.File.Exists(manifestPath))
            {
                manifests = _persistenceService.LoadObject<List<MapManifest>>(manifestPath, false);
            }
            var manifest = new MapManifest(mod, mod.Modfile, installLocation);
            manifests.Add(manifest);
            string mapManifestPath = Path.Combine(installLocation, Manifest.FileName);
            _persistenceService.SaveObject(manifests, mapManifestPath);
            _installedAddonRepository.Add(manifest);

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            _installedAddonRepository.Remove(manifest.ModId);

            return true;
        }

        public bool ChangeVersion(Mod mod, File file, string zipLocation)
        {
            throw new System.NotImplementedException();
        }
    }
}