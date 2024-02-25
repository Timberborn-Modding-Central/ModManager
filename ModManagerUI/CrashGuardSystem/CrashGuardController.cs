using System;
using System.Collections.Generic;
using System.Linq;
using ModManager.AddonSystem;
using ModManager.LoggingSystem;
using ModManager.MapSystem;
using ModManagerUI.UiSystem;
using Timberborn.ErrorReportingUI;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ModManagerUI.CrashGuardSystem
{
    public class CrashGuardController : ILoadableSingleton
    {
        private readonly InstalledAddonRepository _installedAddonRepository = InstalledAddonRepository.Instance;
        private readonly SceneLoader _sceneLoader;

        private static bool _active = true;

        private CrashGuardController(
            SceneLoader sceneLoader, 
            IModManagerLogger modManagerLogger)
        {
            _sceneLoader = sceneLoader;
            
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
            ModManagerUIPlugin.Log.LogInfo("Crash Guard System is now disabled.");
            _active = false;
        }
        
        private void OnSceneLoaded(object sender, EventArgs e)
        {
            if (!_active || SceneManager.GetActiveScene().buildIndex != 2)
                return;

            var obj = new GameObject();
            Object.Instantiate(obj);
            obj.AddComponent<CrashScreenUpdater>();
            
            var crashScreenPanels = Resources.FindObjectsOfTypeAll<CrashScreenPanel>();
            var crashScreenPanel = crashScreenPanels.First();
            var uiDocument = crashScreenPanel._uiDocument;
            var container = uiDocument.panelSettings.visualTree;
            
            var modIds = new List<uint>();
            foreach (var manifest in _installedAddonRepository.All().OrderBy(manifest => manifest.ModName))
            {
                if (manifest is MapManifest)
                    continue;
                if (ModHelper.ContainsBepInEx(manifest) || ModHelper.IsModManager(manifest))
                    continue;
                modIds.Add(manifest.ModId);
            }
            
            container.Add(CrashScreenBox.Instance.Create(modIds));
        }
    }
}