using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModManager.AddonSystem;
using ModManager.LoggingSystem;
using ModManager.ManifestLocationFinderSystem;
using ModManager.PersistenceSystem;
using Newtonsoft.Json;

namespace ModManager.MapSystem
{

    public class MapManifestFinder : IManifestLocationFinder
    {
        private readonly PersistenceService _persistenceService;

        private IModManagerLogger _logger;

        public MapManifestFinder(IModManagerLogger logger)
        {
            _persistenceService = PersistenceService.Instance;
            _logger = logger;
        }

        public IEnumerable<Manifest> Find()
        {
            var manifestPath = Path.Combine(Paths.Maps, MapManifest.FileName);

            if (!File.Exists(manifestPath))
            {
                return new List<MapManifest>();
            }

            try
            {
                var manifests = _persistenceService.LoadObject<List<MapManifest>>(manifestPath, false);
                UpdateManifestInfo(manifests);
                return manifests;
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogWarning($"Failed to serialize JSON in file {manifestPath}. It is advised to delete the file.");
                return new List<MapManifest>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Manifest> FindRemovable()
        {
            throw new NotImplementedException();
        }

        private void UpdateManifestInfo(List<MapManifest> manifests)
        {
            foreach (var mapManifest in manifests)
            {
                mapManifest.RootPath = Paths.Maps;

                var mapNames = mapManifest.MapFileNames
                                          .Select(mapname => mapname + Names.Extensions.TimberbornMap);
                var enabled = true;
                foreach (var mapName in mapNames)
                {
                    if (Directory.GetFiles(Paths.Maps, mapName).FirstOrDefault()?.EndsWith(Names.Extensions.Disabled) ?? false)
                    {
                        enabled = false;
                        break;
                    }
                }
                mapManifest.Enabled = enabled;
            }
        }
    }
}