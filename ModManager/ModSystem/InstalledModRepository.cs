using System.Collections.Generic;
using Modio.Models;
using Newtonsoft.Json;

namespace ModManager.ModSystem
{
    public class InstalledModRepository
    {
        private readonly Dictionary<uint, InstalledMod> _installedMods;

        public InstalledModRepository(Dictionary<uint, InstalledMod> installedMods)
        {
            _installedMods = installedMods;
        }

        public bool TryGet(uint modId, out InstalledMod? mod)
        {
            mod = null;

            if (!Has(modId))
            {
                return false;
            }

            mod = _installedMods[modId];
            return true;
        }

        public InstalledMod Get(uint modId)
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
            _installedMods.Add(mod.Id, InstalledMod.Create(mod));
        }

        public IEnumerable<InstalledMod> All()
        {
            return _installedMods.Values;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(_installedMods);
        }
    }
}