using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModManager.AddonSystem;
using ModManager.ManifestLocationFinderSystem;
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

            UpdateManifestInfo(manifests);

            return manifests;
        }

        public IEnumerable<Manifest> FindRemovable()
        {
            throw new NotImplementedException();
        }

        private void UpdateManifestInfo(List<MapManifest> manifests)
        {
            foreach (MapManifest mapManifest in manifests)
            {
                mapManifest.RootPath = Paths.Maps;
                var mapFullname = $"{mapManifest.MapFileName}{Names.Extensions.TimberbornMap}";
                var mapFile = Directory.GetFiles(Paths.Maps, mapFullname).FirstOrDefault();
                mapManifest.Enabled = mapFile?.EndsWith(Names.Extensions.Disabled) ?? true
                    ? false
                    : true;
            }
        }
    }
}