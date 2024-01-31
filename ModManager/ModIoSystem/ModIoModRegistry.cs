using System.Collections.Generic;
using System.Threading.Tasks;
using Modio.Models;
using Timberborn.Common;

namespace ModManager.ModIoSystem
{
    public abstract class ModIoModRegistry
    {
        private static readonly Dictionary<uint, Mod> ModCache = new();

        public static Mod Get(uint modId)
        {
            return ModCache.GetOrAdd(modId, () => Task.Run(() => RetrieveMod(modId)).Result);
        }
        
        public static Mod Get(Dependency dependency)
        {
            return ModCache.GetOrAdd(dependency.ModId, () => Task.Run(() => RetrieveMod(dependency.ModId)).Result);
        }

        private static async Task<Mod> RetrieveMod(uint modId)
        {
            return await ModIo.GameClient.Mods[modId].Get();
        }
    }
}