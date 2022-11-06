using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModManager.EnableSystem;
using ModManager.ManifestFinderSystem.ManifestLocationFinders;
using ModManager.ModSystem;
using ModManager.PersistenceSystem;
using ModManager.SingletonInstanceSystem;

namespace ModManager.ManifestFinderSystem
{
    public class ManifestFinderService : Singleton<ManifestFinderService>
    {
        private readonly PersistenceService _persistenceService;

        private readonly List<IManifestLocationFinder> _manifestLocationFinders = new()
        {
            new ModManifestFinder(),
            new MapManifestFinder()
        };

        public ManifestFinderService()
        {
            _persistenceService = PersistenceService.Instance;
        }

        public IEnumerable<Manifest> FindAll()
        {
            foreach (string manifestPath in _manifestLocationFinders.SelectMany(manifestLocationFinder => manifestLocationFinder.Find()))
            {
                Manifest manifest;

                try
                {
                    manifest = _persistenceService.LoadObject<Manifest>(manifestPath);

                    manifest.RootPath = Directory.GetDirectoryRoot(manifestPath);

                    manifest.Enabled = Path.GetExtension(manifestPath) != ModEnabler.DisabledExtension;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Skipped manifest at `{manifestPath}`. Reason of failure: {e.Message}");
                    continue;
                }

                yield return manifest;
            }
        }

        public void AddLocationFinder(IManifestLocationFinder locationLocationFinder)
        {
            _manifestLocationFinders.Insert(0, locationLocationFinder);
        }
    }
}