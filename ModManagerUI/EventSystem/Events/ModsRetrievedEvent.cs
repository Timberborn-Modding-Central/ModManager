using System.Collections.Generic;
using System.Threading;
using Modio.Models;

namespace ModManagerUI.EventSystem
{
    public class ModsRetrievedEvent
    {
        public IReadOnlyCollection<Mod> Mods { get; }
        public CancellationToken Token { get; }
        
        public ModsRetrievedEvent(IReadOnlyCollection<Mod> mods, CancellationToken token)
        {
            Mods = mods;
            Token = token;
        }
    }
}