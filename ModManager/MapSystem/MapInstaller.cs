using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;
using ModManager.ExtractorSystem;
using ModManager.PersistenceSystem;
using ModManager.StartupSystem;
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

        private readonly AddonExtractorService _extractor;

        private readonly MapManifestFinder _mapManifestFinder;

        public MapInstaller(ModManagerStartupOptions startupOptions)
        {
            _persistenceService = PersistenceService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
            _extractor = AddonExtractorService.Instance;
            _mapManifestFinder = new MapManifestFinder(startupOptions.Logger);
        }

        public bool Install(Mod mod, string zipLocation)
        {
            if(!mod.Tags.Any(x => x.Name == "Map"))
            {
                return false;
            }
            List<string> timberFileNames = new();
            using (var zipFile = ZipFile.OpenRead(zipLocation))
            {
                timberFileNames = zipFile.Entries
                                         .Where(x => x.Name.Contains(".timber"))
                                         .Select(x => x.Name.Replace(Names.Extensions.TimberbornMap, ""))
                                         .ToList();
            }

            for(var i = 0; i < timberFileNames.Count(); i++) 
            {
                var files = Directory.GetFiles(Paths.Maps, timberFileNames[i]);
                if(files.Length > 0)
                {
                    timberFileNames[i] += $"_{files.Length + 1}";
                }
            }
            var installLocation = _extractor.Extract(mod, zipLocation);

            var manifest = new MapManifest(mod, 
                                           mod.Modfile, 
                                           installLocation, 
                                           timberFileNames);
            var manifests = _mapManifestFinder.Find()
                                              .Select(a => (MapManifest)a)
                                              .ToList();
            manifests.Add(manifest);

            var mapManifestPath = Path.Combine(installLocation, MapManifest.FileName);
            _persistenceService.SaveObject(manifests, mapManifestPath);
            _installedAddonRepository.Add(manifest);

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            if (manifest is not MapManifest)
            {
                return false;
            }

            var manifestPath = Path.Combine(Paths.Maps, MapManifest.FileName);
            var manifests = _mapManifestFinder.Find().Select(a => (MapManifest)a).ToList();
            manifests.Remove(manifests.Where(x => x.ModId == manifest.ModId).SingleOrDefault());
            _persistenceService.SaveObject(manifests, manifestPath);

            _installedAddonRepository.Remove(manifest.ModId);

            foreach(var mapFileName in ((MapManifest)manifest).MapFileNames)
            {
                var files = Directory.GetFiles(Paths.Maps, $"{mapFileName}{Names.Extensions.TimberbornMap}*");
                foreach(var file in files)
                {
                    System.IO.File.Delete(file);
                }
            }

            return true;
        }

        public bool ChangeVersion(Mod mod, File file, string zipLocation)
        {
            if (!mod.Tags.Any(x => x.Name == "Map"))
            {
                return false;
            }

            List<string> timberFileNames = new();
            using (var zipFile = ZipFile.OpenRead(zipLocation))
            {
                timberFileNames = zipFile.Entries
                                         .Where(x => x.Name.Contains(".timber"))
                                         .Select(x => x.Name.Replace(Names.Extensions.TimberbornMap, ""))
                                         .ToList();
            }

            for (var i = 0; i < timberFileNames.Count(); i++)
            {
                var files = Directory.GetFiles(Paths.Maps, timberFileNames[i]);
                if (files.Length > 0)
                {
                    timberFileNames[i] += $"_{files.Length + 1}";
                }
            }
            var installLocation = _extractor.Extract(mod, zipLocation);

            var manifest = new MapManifest(mod,
                                           mod.Modfile,
                                           installLocation,
                                           timberFileNames);
            var manifests = _mapManifestFinder.Find()
                                              .Where(a => a.ModId != mod.Id)
                                              .Select(a => (MapManifest)a)
                                              .ToList();
            manifests.Add(manifest);

            var mapManifestPath = Path.Combine(installLocation, MapManifest.FileName);
            _persistenceService.SaveObject(manifests, mapManifestPath);

            _installedAddonRepository.Remove(manifest.ModId);
            _installedAddonRepository.Add(manifest);

            return true;
        }
    }
}