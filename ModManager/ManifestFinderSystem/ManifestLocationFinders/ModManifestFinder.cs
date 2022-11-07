using System.Collections.Generic;
using System.IO;
using ModManager.EnableSystem;
using ModManager.ModSystem;
using ModManager.PersistenceSystem;

namespace ModManager.ManifestFinderSystem.ManifestLocationFinders
{
    public class ModManifestFinder : IManifestLocationFinder
    {
        private readonly PersistenceService _persistenceService;

        public ModManifestFinder()
        {
            _persistenceService = PersistenceService.Instance;
        }

        public IEnumerable<Manifest> Find()
        {
            foreach (string enabledManifest in Directory.GetFiles(Paths.Mods, Manifest.FileName, SearchOption.AllDirectories))
            {
                yield return LoadManifest(enabledManifest);
            }

            foreach (string disabledManifest in Directory.GetFiles(Paths.Mods, Manifest.FileName + ModEnableService.DisabledExtension, SearchOption.AllDirectories))
            {
                yield return LoadManifest(disabledManifest);
            }
        }

        private Manifest LoadManifest(string manifestPath)
        {
            var manifest = _persistenceService.LoadObject<Manifest>(manifestPath);

            manifest.RootPath = Path.GetDirectoryName(manifestPath)!;

            return manifest;
        }
    }
}