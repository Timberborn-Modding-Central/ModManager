using Modio.Models;
using ModManager.AddonSystem;

namespace ModManagerUI.UiSystem
{
    public abstract class ModHelper
    {
        public static uint ModManagerId => 2541476;
        
        public static bool ContainsBepInEx(Mod mod)
        {
            return !string.IsNullOrEmpty(mod.Name) && ContainsBepInEx(mod.Name);
        }
        
        public static bool ContainsBepInEx(Manifest manifest)
        {
            return ContainsBepInEx(manifest.ModName);
        }

        private static bool ContainsBepInEx(string name)
        {
            return name.ToLower().Contains("BepInEx".ToLower());
        }

        public static bool IsModManager(Mod mod)
        {
            return mod.Id == ModManagerId;
        }
        
        public static bool IsModManager(Manifest manifest)
        {
            return manifest.ModId == ModManagerId;
        }
    }
}