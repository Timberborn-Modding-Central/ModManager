using System.Collections.Generic;
using System.Linq;
using ModManager.ManifestFinderSystem.ManifestLocationFinders;
using ModManager.ModSystem;
using ModManager.SingletonInstanceSystem;

namespace ModManager.ManifestFinderSystem
{
    public class ManifestFinderService : Singleton<ManifestFinderService>
    {
        private readonly List<IManifestLocationFinder> _manifestLocationFinders = new()
        {
            new ModManifestFinder(),
            new MapManifestFinder()
        };

        public IEnumerable<Manifest> FindAll()
        {
            return _manifestLocationFinders.SelectMany(manifestLocationFinder => manifestLocationFinder.Find());
        }

        public void AddLocationFinder(IManifestLocationFinder locationLocationFinder)
        {
            _manifestLocationFinders.Insert(0, locationLocationFinder);
        }
    }
}