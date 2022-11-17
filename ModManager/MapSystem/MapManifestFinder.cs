using System.Collections.Generic;
using System.IO;
using ModManager.AddonSystem;
using ModManager.ManifestLocationFinderSystem;
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

            List<MapManifest> manifests = _persistenceService.LoadObject<List<MapManifest>>(manifestPath, false);

            SetEnabledStatus(manifests);

            return manifests;
        }

        private void SetEnabledStatus(List<MapManifest> manifests)
        {
            foreach (MapManifest mapManifest in manifests)
            {

            }
        }
    }
}