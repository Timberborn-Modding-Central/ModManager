using System.Collections.Generic;
using System.Threading.Tasks;
using Modio.Models;
using Timberborn.Common;

namespace ModManager.ModIoSystem
{
    public abstract class ModIoModDependenciesRegistry
    {
        private static readonly Dictionary<uint, IReadOnlyList<Dependency>> ModDependenciesCache = new();

        public static IReadOnlyList<Dependency> Get(uint modId)
        {
            return ModDependenciesCache.GetOrAdd(modId, () => Task.Run(() => RetrieveMod(modId)).Result);
        }
        
        public static IReadOnlyList<Dependency> Get(Mod mod)
        {
            return ModDependenciesCache.GetOrAdd(mod.Id, () => Task.Run(() => RetrieveMod(mod.Id)).Result);
        }
        
        public static IReadOnlyList<Dependency> Get(Dependency dependency)
        {
            return ModDependenciesCache.GetOrAdd(dependency.ModId, () => Task.Run(() => RetrieveMod(dependency.ModId)).Result);
        }

        private static async Task<IReadOnlyList<Dependency>> RetrieveMod(uint modId)
        {
            return await ModIo.ModsClient[modId].Dependencies.Get();
        }
    }
}