using System;
using ModManager.AddonEnableSystem;
using ModManager.AddonSystem;
using ModManager.LoggingSystem;
using ModManager.MapSystem;
using ModManagerUI.UiSystem;
using Timberborn.CoreUI;
using Timberborn.SceneLoading;
using UnityEngine.SceneManagement;

namespace ModManagerUI.CrashGuardSystem
{
    public class CrashGuardController
    {
        public static IModManagerLogger ModManagerLogger;
        
        private readonly InstalledAddonRepository _installedAddonRepository = InstalledAddonRepository.Instance;
        private readonly AddonEnablerService _addonEnablerService = AddonEnablerService.Instance;
        private readonly IModManagerLogger _modManagerLogger;
        private readonly CrashScreenBox _crashScreenBox;
        private readonly DialogBoxShower _dialogBoxShower;
        private readonly SceneLoader _sceneLoader;

        private static bool _hasPassedLoading;

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
        }

        private void OnSceneLoaded(object sender, EventArgs e)
        {
            if (_hasPassedLoading || SceneManager.GetActiveScene().buildIndex != 2)
            {
                _hasPassedLoading = true;
                return;
            }

            _modManagerLogger.LogWarning("IMPORTANT: it seems the game crashed while trying to load into the game. All mods are being disabled.");
            DisableAllMods();
            _sceneLoader.LoadScene(0, 0, null);
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