using System.Linq;
using Modio.Models;
using ModManager.AddonSystem;
using ModManager.VersionSystem;
using ModManagerUI.EventSystem;
using ModManagerUI.UiSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;
using EventBus = ModManagerUI.EventSystem.EventBus;

namespace ModManagerUI.Components.ModCard
{
    public class DownloadButton
    {
        private readonly Button _root;
        private readonly Mod _mod;
        
        public DownloadButton(Button root, Mod mod)
        {
            _root = root;
            _mod = mod;
        }

        public void Initialize()
        {
            EventBus.Instance.Register(this);
            _root.clicked += async () =>
            {
                Disable();
                await InstallController.DownloadAndExtractWithDependencies(_mod);
            };
            Refresh();
        }

        [OnEvent]
        public void OnUpdatableModsRetrieved(UpdatableModsRetrievedEvent updatableModsRetrievedEvent)
        {
            Refresh();
        }
        
        public void Enable()
        {
            _root.SetEnabled(true);
        }
        
        public void Disable()
        {
            _root.SetEnabled(false);
        }

        public void Refresh()
        {
            _root.text = TextGetter();
            if (_mod.Modfile == null)
            {
                _root.SetEnabled(false);
                return;
            }

            if (InstalledAddonRepository.Instance.TryGet(_mod.Id, out var manifest))
            {
                var isSameVersion = VersionComparer.IsSameVersion(manifest.Version, _mod.Modfile.Version);
                _root.SetEnabled(!isSameVersion);
            }
            else
            {
                _root.SetEnabled(true);
            }
        }

        private string TextGetter()
        {
            if (UpdateableModRegistry.UpdateAvailable == null)
                return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Download");
            if (UpdateableModRegistry.UpdateAvailable.Values.Any(file => file.ModId == _mod.Id))
                return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Update");
            return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Download");
        }
    }
}