using System.Collections.Generic;
using ModManager.AddonSystem;

namespace ModManager.ManifestLocationFinderSystem
{
    public interface IManifestLocationFinder
    {
        public IEnumerable<Manifest> Find();

        IEnumerable<Manifest> FindRemovable();
    }
}