using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ModManager.AddonSystem;
using ModManager.ManifestLocationFinderSystem;
using ModManager.PersistenceSystem;

namespace ModManager.ModSystem
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

            foreach (string disabledManifest in Directory.GetFiles(Paths.Mods, Manifest.FileName + Names.Extensions.Disabled, SearchOption.AllDirectories))
            {
                yield return LoadManifest(disabledManifest);
            }

            foreach (string enabledManifest in Directory.GetFiles(Paths.Mods, Manifest.FileName + Names.Extensions.Remove, SearchOption.AllDirectories))
            {
                yield return LoadManifest(enabledManifest);
            }
        }

        public IEnumerable<Manifest> FindRemovable()
        {
            return new List<Manifest>();
        }

        private Manifest LoadManifest(string manifestPath)
        {
            try
            {
                var manifest = _persistenceService.LoadObject<Manifest>(manifestPath, false);

                manifest.Enabled = !Path.GetExtension(manifestPath).Equals(Names.Extensions.Disabled);

                manifest.RootPath = Path.GetDirectoryName(manifestPath)!;

                return manifest;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}