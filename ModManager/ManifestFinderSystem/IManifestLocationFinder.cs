using System.Collections.Generic;
using ModManager.ModSystem;

namespace ModManager.ManifestFinderSystem
{
    public interface IManifestLocationFinder
    {
        public IEnumerable<Manifest> Find();
    }
}