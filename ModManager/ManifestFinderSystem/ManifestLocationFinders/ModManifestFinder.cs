using System.Collections.Generic;
using System.IO;
using ModManager.EnableSystem;
using ModManager.ModSystem;

namespace ModManager.ManifestFinderSystem.ManifestLocationFinders
{
    public class ModManifestFinder : IManifestLocationFinder
    {
        public IEnumerable<string> Find()
        {
            foreach (string enabledManifests in Directory.GetFiles(Paths.Mods, Manifest.FileName, SearchOption.AllDirectories))
            {
                yield return enabledManifests;
            }

            foreach (string disabledManifests in Directory.GetFiles(Paths.Mods, Manifest.FileName + ModEnabler.DisabledExtension, SearchOption.AllDirectories))
            {
                yield return disabledManifests;
            }
        }
    }
}