using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using ModManagerWrapper.StartupSystem;

namespace ModManagerWrapper.ModSystem
{
    public class InstalledModRepository : ILoadable
    {
        public InstalledModRepository Instance { get; private set; } = null!;

        private Dictionary<uint, Manifest> _installedMods;

        private readonly InstalledModLoader _installedModLoader;

        public InstalledModRepository()
        {
            _installedModLoader = new InstalledModLoader();
            _installedMods = new Dictionary<uint, Manifest>();
        }

        public bool TryGet(uint modId, out Manifest? mod)
        {
            mod = null;

            if (!_installedMods.ContainsKey(modId))
            {
                return false;
            }

            mod = _installedMods[modId];
            return true;
        }

        public Manifest Get(uint modId)
        {
            return _installedMods[modId];
        }

        public void Remove(uint modId)
        {
            _installedMods.Remove(modId);
        }

        public Manifest Add(Mod mod)
        {
            var manifest = Manifest.Create(mod);

            _installedMods.Add(mod.Id, manifest);

            return manifest;
        }

        public IEnumerable<Manifest> All()
        {
            return _installedMods.Values;
        }

        public void Load(ModManagerStartupOptions startupOptions)
        {
            Instance = this;
            _installedMods = _installedModLoader.GetAllInstalledManifests().ToDictionary(manifest => manifest.ModId);
        }
    }
}