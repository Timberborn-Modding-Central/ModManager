using System.Collections.Generic;
using System.Linq;
using ModManager.ManifestLocationFinderSystem;
using ModManager.StartupSystem;

namespace ModManager.AddonSystem
{
    public class InstalledAddonRepository : Singleton<InstalledAddonRepository>, ILoadable
    {
        private Dictionary<uint, Manifest> _installedMods;

        private readonly ManifestLocationFinderService _manifestLocationFinderService;

        public InstalledAddonRepository()
        {
            _manifestLocationFinderService = ManifestLocationFinderService.Instance;
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
            _installedMods = _manifestLocationFinderService.FindAll().ToDictionary(manifest => manifest.ModId);
        }
    }
}