using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModManager.AddonSystem;
using ModManager.MapSystem;
using ModManager.ModIoSystem;
using ModManagerUI.EventSystem;
using ModManagerUI.UiSystem;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;
using EventBus = ModManagerUI.EventSystem.EventBus;
using File = Modio.Models.File;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class UpdateAllWrapper
    {
        private readonly VisualElement _root;
        private readonly Func<Dictionary<uint, File>> _updateAvailableGetter;
        
        private readonly Label _updateAllLabel;
        private readonly Button _updateAllButton;
        
        private UpdateAllWrapper(
            VisualElement root, 
            Func<Dictionary<uint, File>> updateAvailableGetter, 
            Label updateAllLabel, 
            Button updateAllButton)
        {
            _root = root;
            _updateAvailableGetter = updateAvailableGetter;
            _updateAllLabel = updateAllLabel;
            _updateAllButton = updateAllButton;
        }

        public static UpdateAllWrapper Create(VisualElement root, Func<Dictionary<uint, File>> updateAvailableGetter)
        {
            var updateAllLabel = root.Q<Label>("UpdateAllLabel");
            var updateAllButton = root.Q<Button>("UpdateAll");
            var updateAllWrapper = new UpdateAllWrapper(root, updateAvailableGetter, updateAllLabel, updateAllButton);
            updateAllButton.RegisterCallback<ClickEvent>(updateAllWrapper.OnButtonClickEvent);
            root.ToggleDisplayStyle(false);
            EventBus.Instance.Register(updateAllWrapper);
            return updateAllWrapper;
        }

        [OnEvent]
        public void OnUpdatableModsRetrieved(UpdatableModsRetrievedEvent updatableModsRetrievedEvent)
        {
            Refresh();
        }

        private void Refresh()
        {
            var number = _updateAvailableGetter()?.Count;
            _updateAllLabel.text = UiSystem.ModManagerPanel.Loc.T("Mods.UpdateAllLabel", number);
            _root.ToggleDisplayStyle(number > 0);
        }

        private async void OnButtonClickEvent(ClickEvent _)
        {
            await UpdateUpdatableMods();
        }
        
        private async Task UpdateUpdatableMods()
        {
            _updateAllButton.SetEnabled(false);
            UiSystem.ModManagerPanel.ModsWereChanged = true;
            foreach (var updatableMod in _updateAvailableGetter().ToArray())
            {
                try
                {
                    var mod = ModIoModRegistry.Get(updatableMod.Value.ModId);
                    await InstallController.DownloadAndExtract(mod, updatableMod.Value);
                    _updateAvailableGetter().Remove(updatableMod.Key);
                }
                catch (MapException ex)
                {
                    ModManagerUIPlugin.Log.LogWarning(ex.Message);
                }
                catch (AddonException ex)
                {
                    ModManagerUIPlugin.Log.LogWarning(ex.Message);
                }
                catch (IOException ex)
                {
                    ModManagerUIPlugin.Log.LogError($"{ex.Message}");
                }
            }
            
            _updateAllButton.SetEnabled(true);
            EventBus.Instance.PostEvent(new UpdatableModsRetrievedEvent(_updateAvailableGetter()));
        }
    }
}