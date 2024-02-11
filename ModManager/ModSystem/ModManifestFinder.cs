using System;
using System.Collections.Generic;
using System.IO;
using ModManager.AddonSystem;
using ModManager.LoggingSystem;
using ModManager.ManifestLocationFinderSystem;
using ModManager.PersistenceSystem;
using Newtonsoft.Json;

namespace ModManager.ModSystem
{
    public class ModManifestFinder : IManifestLocationFinder
    {
        private readonly PersistenceService _persistenceService;

        private IModManagerLogger _logger;

        public ModManifestFinder(IModManagerLogger logger)
        {
            _persistenceService = PersistenceService.Instance;
            _logger = logger;
        }

        public IEnumerable<Manifest> Find()
        {

            foreach (var enabledManifest in Directory.GetFiles(Paths.Mods, Manifest.FileName, SearchOption.AllDirectories))
            {
                var manifest = LoadManifest(enabledManifest);
                if (manifest == null)
                { 
                    continue; 
                }
                yield return manifest;

            }

            foreach (var disabledManifest in Directory.GetFiles(Paths.Mods, Manifest.FileName + Names.Extensions.Disabled, SearchOption.AllDirectories))
            {
                var manifest = LoadManifest(disabledManifest);
                if (manifest == null)
                {
                    continue;
                }
                yield return manifest;
            }

            foreach (var enabledManifest in Directory.GetFiles(Paths.Mods, Manifest.FileName + Names.Extensions.Remove, SearchOption.AllDirectories))
            {
                var manifest = LoadManifest(enabledManifest);
                if (manifest == null)
                {
                    continue;
                }
                yield return manifest;
            }
        }

        public IEnumerable<Manifest> FindRemovable()
        {
            return new List<Manifest>();
        }

        private Manifest? LoadManifest(string manifestPath)
        {
            try
            {
                var manifest = _persistenceService.LoadObject<Manifest>(manifestPath, false);
                manifest.Enabled = !Path.GetExtension(manifestPath).Equals(Names.Extensions.Disabled);
                manifest.RootPath = Path.GetDirectoryName(manifestPath)!;
                return manifest;
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogWarning($"Failed to serialize JSON in file {manifestPath} It is advised to delete the file.");
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}