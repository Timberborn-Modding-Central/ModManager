using BepInEx.Configuration;

namespace ModManagerUI.CrashGuardSystem
{
    public abstract class CrashGuardSystemConfig
    {
        public static ConfigEntry<bool> CrashGuardEnabled { get; private set; }

        public static void Initialize(ConfigFile configFile)
        {
            CrashGuardEnabled = configFile.Bind(
                "Settings", 
                "CrashGuardEnabled", 
                true, 
                "Determines whether the CrashGuardSystem is enabled. This system disables all mods upon crash.");
        }
    }
}