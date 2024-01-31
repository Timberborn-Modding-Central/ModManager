using Modio.Models;

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
    }
}