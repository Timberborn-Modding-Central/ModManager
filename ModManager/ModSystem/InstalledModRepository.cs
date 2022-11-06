using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using ModManager.ManifestFinderSystem;
using ModManager.SingletonInstanceSystem;
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

        public bool TryGet(Mod mod, out Manifest manifest)
        {
            return _installedMods.TryGetValue(mod.Id, out manifest);
        }

        public Manifest Get(Mod mod)
        {
            return _installedMods[mod.Id];
        }

        public bool Has(Mod mod)
        {
            return _installedMods.ContainsKey(mod.Id);
        }

        public void Remove(Mod mod)
        {
            _installedMods.Remove(mod.Id);
        }

        public Manifest Add(Mod mod, File file, string installationPath)
        {
            var manifest = new Manifest(mod, file, installationPath);

            _installedMods.Add(mod.Id, manifest);

            return manifest;
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