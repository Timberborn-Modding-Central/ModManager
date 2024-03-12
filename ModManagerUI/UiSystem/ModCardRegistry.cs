using System.Collections.Generic;
using System.Linq;
using Modio.Models;

namespace ModManagerUI.UiSystem
{
    public abstract class ModCardRegistry
    {
        public static readonly List<ModCard> ModCards = new();

        public static ModCard? Get(Mod mod)
        {
            return Get(mod.Id);
        }
        
        public static ModCard? Get(uint modId)
        {
            return ModCards.FirstOrDefault(card => card.Mod.Id == modId);
        }
    }
}