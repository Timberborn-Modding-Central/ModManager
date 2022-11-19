using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManager.PersistenceSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using File = Modio.Models.File;

namespace ModManager.MapSystem
{
    public class MapInstaller : IAddonInstaller
    {
        private readonly PersistenceService _persistenceService;

        private readonly InstalledAddonRepository _installedAddonRepository;

        private readonly ExtractorService _extractor;

        private readonly MapManifestFinder _mapManifestFinder;

        public MapInstaller()
        {
            _persistenceService = PersistenceService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
            _extractor = ExtractorService.Instance;
            _mapManifestFinder = new MapManifestFinder();
        }

        public bool Install(Mod mod, string zipLocation)
        {
            if(!mod.Tags.Any(x => x.Name == "Map"))
            {
                return false;
            }
            var zipFile = ZipFile.OpenRead(zipLocation);
            var timberFileName = zipFile.Entries
                                    .Where(x => x.Name.Contains(".timber"))
                                    .Single()
                                    .Name;
            zipFile.Dispose();
            string installLocation = _extractor.ExtractMap(zipLocation, mod);

            // TODO: manifest handling is probably wrong atm
            var manifest = new MapManifest(mod, mod.Modfile, installLocation, timberFileName);

            var manifests = _mapManifestFinder.Find().Select(a => (MapManifest)a).ToList();
            manifests.Add(manifest);

            string mapManifestPath = Path.Combine(installLocation, Manifest.FileName);
            _persistenceService.SaveObject(manifests, mapManifestPath);
            _installedAddonRepository.Add(manifest);

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            Console.WriteLine($"inside mapinstaller");
            if (manifest is not MapManifest)
            {
                Console.WriteLine($"\"{manifest.ModName} is not a map.");
                return false;
            }

            string manifestPath = Path.Combine(Paths.Maps, Manifest.FileName);
            var manifests = _mapManifestFinder.Find().Select(a => (MapManifest)a).ToList();
            manifests.Remove(manifests.Where(x => x.ModId == manifest.ModId).SingleOrDefault());
            _persistenceService.SaveObject(manifests, manifestPath);

            _installedAddonRepository.Remove(manifest.ModId);

            var mapFullPath = Path.Combine(Paths.Maps, ((MapManifest)manifest).MapFileName);
            System.IO.File.Delete(mapFullPath);

            return true;
        }

        public bool ChangeVersion(Mod mod, File file, string zipLocation)
        {
            throw new System.NotImplementedException();
        }
    }
}