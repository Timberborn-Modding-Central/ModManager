using System.Collections.Generic;
using ModManager.ModSystem;

namespace ModManager.ManifestFinderSystem.ManifestLocationFinders
{
    public class MapManifestFinder : IManifestLocationFinder
    {
        public IEnumerable<Manifest> Find()
        {
            return new List<Manifest>();
        }
    }
}