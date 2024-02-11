using Modio.Models;
using ModManager.AddonSystem;

namespace ModManagerUI.UiSystem
{
    public abstract class ModHelper
    {
        public static bool ContainsBepInEx(Mod mod)
        {
            return mod.Name!.ToLower().Contains("BepInEx".ToLower());
        }

        public static bool IsModManager(Mod mod)
        {
            return mod.Id == 2541476;
        }
        
        public static bool IsModManager(Manifest manifest)
        {
            return manifest.ModId == 2541476;
        }
    }
}