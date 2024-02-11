using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Modio.Models;
using ModManager.AddonSystem;
using ModManager.VersionSystem;
using ModManagerUI.EventSystem;
using Timberborn.SingletonSystem;
using EventBus = ModManagerUI.EventSystem.EventBus;

namespace ModManagerUI.UiSystem
{
    public class UpdateableModRegistry : ILoadableSingleton
    {
        public static Dictionary<uint, File>? UpdateAvailable { get; private set; }

        public void Load()
        {
            EventBus.Instance.Register(this);
        }
        
        [OnEvent]
        public void OnModManagerPanelOpenedEvent(ModManagerPanelRefreshEvent modManagerPanelRefreshEvent)
        {
            try
            {
                Task.Run(IndexUpdatableMods);
            }
            catch (OperationCanceledException ex)
            {
                ModManagerUIPlugin.Log.LogDebug($"Async operation was cancelled: {ex.Message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                throw;
            }
        }
        
        private static async Task IndexUpdatableMods()
        {
            if (UpdateAvailable != null)
                return;
            UpdateAvailable = new Dictionary<uint, File>();
            var installedMods = InstalledAddonRepository.Instance.All().ToList();
            foreach (var manifest in installedMods)
            {
                File? file;
                try
                {
                    file = await AddonService.Instance.TryGetCompatibleVersion(manifest.ModId, ModManagerPanel.CheckForHighestInsteadOfLive);
                }
                catch (Exception)
                {
                    continue;
                }

                if (file == null)
                    continue;
                
                if (file.Version != manifest.Version && VersionComparer.IsVersionHigher(file.Version, manifest.Version))
                {
                    UpdateAvailable.Add(file.Id, file);
                }
            }
            
            EventBus.Instance.PostEvent(new UpdatableModsRetrievedEvent(UpdateAvailable)); 
        }
    }
}