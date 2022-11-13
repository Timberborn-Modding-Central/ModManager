using System.Collections.Generic;
using System.Linq;
using ModManager.ManifestFinderSystem;
using ModManager.StartupSystem;

namespace ModManager.ModSystem
{
    public class InstalledModRepository : Singleton<InstalledModRepository>, ILoadable
    {
        private Dictionary<uint, Manifest> _installedMods;

        private readonly ManifestFinderService _manifestFinderService;

        public InstalledModRepository()
        {
            _manifestFinderService = ManifestFinderService.Instance;
            _installedMods = new Dictionary<uint, Manifest>();
        }

        public bool TryGet(uint modId, out Manifest manifest)
        {
            return _installedMods.TryGetValue(modId, out manifest);
        }

        public Manifest Get(uint modId)
        {
            return _installedMods[modId];
        }

        public bool Has(uint modId)
        {
            return _installedMods.ContainsKey(modId);
        }

        public void Remove(uint modId)
        {
            _installedMods.Remove(modId);
        }

        public void Add(Manifest manifest)
        {
            _installedMods.Add(manifest.ModId, manifest);
        }

        public IEnumerable<Manifest> All()
        {
            return _installedMods.Values;
        }

        public void Load(ModManagerStartupOptions startupOptions)
        {
            _installedMods = _manifestFinderService.FindAll().ToDictionary(manifest => manifest.ModId);
        }
    }
}