using System.Collections.Generic;

namespace ModManager.ManifestFinderSystem
{
    public interface IManifestLocationFinder
    {
        public IEnumerable<string> Find();
    }
}