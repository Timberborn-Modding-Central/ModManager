using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManager.PersistenceSystem; 
using Timberborn.SingletonSystem;

namespace ModManagerUI.UiSystem
{
    public class ModManagerRegisterer : ILoadableSingleton
    {
        private readonly PersistenceService _persistenceService = PersistenceService.Instance;
        
        private readonly string _modManagerFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        public void Load()
        {
            foreach (var manifest in InstalledAddonRepository.Instance.All())
            {
                if (manifest.ModId == ModHelper.ModManagerId)
                {
                    return;
                }
            }

            var modFiles = ModIoModFilesRegistry.Get(ModHelper.ModManagerId);
            var file = modFiles.FirstOrDefault(file => file.Version == ModManagerUIPlugin.PluginInfo.Metadata.Version.ToString());

            if (file == null)
            {
                throw new NullReferenceException("The current BepInEx Mod Manager plugin version is not the same as on Mod.io.");
            }
            
            var mod = ModIoModRegistry.Get(ModHelper.ModManagerId);
            var modManagerManifest = new Manifest(mod, file, _modManagerFolderPath);
            var modManifestPath = Path.Combine(_modManagerFolderPath, Manifest.FileName);
            _persistenceService.SaveObject(modManagerManifest, modManifestPath);
            
        }
    }
}