using System.Collections.Generic;
using System.Linq;

namespace ModManager.ManifestLocationFinderSystem
{
    public class ManifestLocationFinderRegistry : Singleton<ManifestLocationFinderRegistry>
    {
        private readonly List<KeyValuePair<string, IManifestLocationFinder>> _manifestLocationFinders = new();

        public void Add(string manifestLocationFinderId, IManifestLocationFinder manifestLocationFinder)
        {
            if (_manifestLocationFinders.Exists(pair => pair.Key.Equals(manifestLocationFinderId)))
            {
                throw new ManifestException($"Manifest location finder with id: `{manifestLocationFinderId}` is already added to the list");
            }

            _manifestLocationFinders.Insert(0, new KeyValuePair<string, IManifestLocationFinder>(manifestLocationFinderId, manifestLocationFinder));
        }

        public void Remove(string manifestLocationFinderId)
        {
            _manifestLocationFinders.Remove(_manifestLocationFinders.First(pair => pair.Key.Equals(manifestLocationFinderId)));
        }

        public IEnumerable<IManifestLocationFinder> GetManifestLocationFinders()
        {
            return _manifestLocationFinders.Select(pair => pair.Value);
        }
    }
}