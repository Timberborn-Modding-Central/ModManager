using System.Collections.Generic;
using System.Linq;
using ModManager.AddonSystem;
using ModManager.MapSystem;
using ModManager.ModSystem;

namespace ModManager.ManifestLocationFinderSystem
{
    public class ManifestLocationFinderService : Singleton<ManifestLocationFinderService>
    {
        private readonly ManifestLocationFinderRegistry _manifestLocationFinderRegistry;

        public ManifestLocationFinderService()
        {
            _manifestLocationFinderRegistry = ManifestLocationFinderRegistry.Instance;
        }

        public IEnumerable<Manifest> FindAll()
        {
            return _manifestLocationFinderRegistry.GetManifestLocationFinders().SelectMany(manifestLocationFinder => manifestLocationFinder.Find());
        }
    }
}