using System;
using ModManager.AddonEnableSystem;
using ModManager.AddonSystem;
using ModManager.LoggingSystem;
using ModManager.MapSystem;
using ModManagerUI.UiSystem;
using Timberborn.CoreUI;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using UnityEngine.SceneManagement;

namespace ModManagerUI.CrashGuardSystem
{
    public class CrashGuardController : ILoadableSingleton
    {
        public static IModManagerLogger ModManagerLogger;
        
        private readonly InstalledAddonRepository _installedAddonRepository = InstalledAddonRepository.Instance;
        private readonly AddonEnablerService _addonEnablerService = AddonEnablerService.Instance;
        private readonly IModManagerLogger _modManagerLogger;
        private readonly CrashScreenBox _crashScreenBox;
        private readonly DialogBoxShower _dialogBoxShower;
        private readonly SceneLoader _sceneLoader;

        private static bool _active = true;

        private CrashGuardController(
            SceneLoader sceneLoader, 
            CrashScreenBox crashScreenBox,
            IModManagerLogger modManagerLogger,
            DialogBoxShower dialogBoxShower)
        {
            _modManagerLogger = modManagerLogger;
            _crashScreenBox = crashScreenBox;
            _dialogBoxShower = dialogBoxShower;
            _sceneLoader = sceneLoader;
            
            ModManagerLogger = modManagerLogger;
            
            _sceneLoader.SceneLoaded += OnSceneLoaded;
            
            if (!CrashGuardSystemConfig.CrashGuardEnabled.Value) 
                Disable();
        }
        
        public void Load()
        {
            // Test
            // throw new Exception();
        }

        public static void Disable()
        {
            if (!_active)
                return;
            ModManagerLogger.LogInfo("Crash Guard System is now disabled.");
            _active = false;
        }
        
        private void OnSceneLoaded(object sender, EventArgs e)
        {
            if (!_active || SceneManager.GetActiveScene().buildIndex != 2)
                return;
            _modManagerLogger.LogWarning("IMPORTANT: it seems the game crashed while trying to load into the game. All mods are being disabled.");
            DisableAllMods();
        }
        
        private void DisableAllMods()
        {
            foreach (var manifest in _installedAddonRepository.All())
            {
                if (manifest is MapManifest)
                    continue;
                if (ModHelper.IsModManager(manifest))
                    continue;
                _addonEnablerService.Disable(manifest);
            }
        }
    }
}