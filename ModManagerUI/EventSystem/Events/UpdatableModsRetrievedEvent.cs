using System.Collections.Generic;
using Modio.Models;

namespace ModManagerUI.EventSystem
{
    public class UpdatableModsRetrievedEvent
    {
        public Dictionary<uint, File> UpdatableMods { get; }
        
        public UpdatableModsRetrievedEvent(Dictionary<uint, File> updatableMods)
        {
            UpdatableMods = updatableMods;
        }
    }
}