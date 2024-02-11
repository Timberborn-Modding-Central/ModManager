using ModManager.ManifestValidatorSystem;
using ModManager.PersistenceSystem;
using ModManager.StartupSystem;
using System.IO;
using System.Linq;

namespace ModManager.MapSystem
{
    public class MapManifestValidator : IManifestValidator
    {
        private readonly PersistenceService _persistenceService;
        private readonly MapManifestFinder _mapManifestFinder;

        public MapManifestValidator(ModManagerStartupOptions startupOptions)
        {
            _persistenceService = PersistenceService.Instance;
            _mapManifestFinder = new MapManifestFinder(startupOptions.Logger);
        }

        public void ValidateManifests()
        {
            var mapManifests = _mapManifestFinder.Find().Select(a => (MapManifest)a).ToList();
            var oldManifestCount = mapManifests.Count;

            foreach (var mapManifest in mapManifests.ToList())
            {
                var fileNames = 
                    mapManifest.MapFileNames
                               .Select(filename => Path.Combine(Paths.Maps, 
                                                         $"{filename}{Names.Extensions.TimberbornMap}"))
                               .ToList();

                var filesDontExist = true;


                foreach(var filename in fileNames)
                {
                    if (File.Exists(filename))
                    {
                        filesDontExist = false;
                    }
                }
                if (filesDontExist)
                {
                    mapManifests.Remove(mapManifest);
                }
            }
            var newManifestCount = mapManifests.Count;

            if(oldManifestCount != newManifestCount )
            {
                var mapManifestPath = Path.Combine(Paths.Maps, MapManifest.FileName);
                _persistenceService.SaveObject(mapManifests, mapManifestPath);
            }
        }
    }
}
