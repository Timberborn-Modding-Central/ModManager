using System.Collections.Generic;
using System.IO;
using ModManager.EnableSystem.Enablers.MapEnablerSystem;
using ModManager.ManifestFinderSystem;
using ModManager.ModSystem;
using ModManager.PersistenceSystem;

namespace ModManager.MapSystem
{
    public class MapManifestFinder : IManifestLocationFinder
    {
        private readonly PersistenceService _persistenceService;


        public MapManifestFinder()
        {
            _persistenceService = PersistenceService.Instance;
        }

        public IEnumerable<Manifest> Find()
        {
            string manifestPath = Path.Combine(Paths.Maps, Manifest.FileName);

            if (! File.Exists(manifestPath))
            {
                return new List<Manifest>();
            }

            return _persistenceService.LoadObject<List<MapManifest>>(manifestPath);
        }
    }
}