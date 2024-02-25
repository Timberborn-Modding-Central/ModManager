using Modio.Models;
using ModManager.AddonSystem;

namespace ModManagerUI.UiSystem
{
    public abstract class EnableController
    {
        public static bool AllowedToChangeState(Manifest manifest)
        {
            if (ModHelper.IsModManager(manifest))
                return false;
            if (ModHelper.ContainsBepInEx(manifest))
                return false;
            return true;
        }
        
        public static bool AllowedToChangeState(Mod mod)
        {
            if (ModHelper.IsModManager(mod))
                return false;
            if (ModHelper.ContainsBepInEx(mod))
                return false;
            return true;
        }
        
        public static void ChangeState(Manifest manifest, bool state)
        {
            if (AllowedToChangeState(manifest))
            {
                ChangeState(manifest.ModId, state);
            }
            else
            {
                ModManagerUIPlugin.Log.LogWarning($"Changing state of {manifest.ModName} is not allowed.");
            }
        }
        
        public static void ChangeState(Mod mod, bool state)
        {
            if (AllowedToChangeState(mod))
            {
                ChangeState(mod.Id, state);
            }
            else
            {
                ModManagerUIPlugin.Log.LogWarning($"Changing state of {mod.Name} is not allowed.");
            }
        }

        private static void ChangeState(uint modId, bool state)
        {
            try
            {
                if (state)
                {
                    AddonService.Instance.Enable(modId);
                }
                else
                {
                    AddonService.Instance.Disable(modId);
                }
                ModManagerPanel.ModsWereChanged = true;
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
        }
    }
}