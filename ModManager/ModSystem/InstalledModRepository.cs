using System.Collections.Generic;
using Modio.Models;
using Newtonsoft.Json;

namespace ModManager.ModSystem
{
    public class InstalledModRepository
    {
        private readonly Dictionary<uint, Manifest> _installedMods;

        public InstalledModRepository(Dictionary<uint, Manifest> installedMods)
        {
            _installedMods = installedMods;
        }

        public bool TryGet(uint modId, out Manifest? mod)
        {
            mod = null;

            if (!Has(modId))
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

        public bool Has(uint modId)
        {
            return _installedMods.ContainsKey(modId);
        }

        public void Remove(uint modId)
        {
            _installedMods.Remove(modId);
        }

        public void Add(Mod mod)
        {
            _installedMods.Add(mod.Id, Manifest.Create(mod));
        }

        public IEnumerable<Manifest> All()
        {
            return _installedMods.Values;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(_installedMods);
        }
    }
}