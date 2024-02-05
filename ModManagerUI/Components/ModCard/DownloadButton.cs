using System;
using System.Threading.Tasks;
using Modio.Models;
using ModManager.AddonSystem;
using ModManager.VersionSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModCard
{
    public class DownloadButton
    {
        private readonly Button _root;
        private readonly Mod _mod;
        
        private Func<string> _valueGetter = null!;
        
        public DownloadButton(Button root, Mod mod)
        {
            _root = root;
            _mod = mod;
        }

        public void Initialize()
        {
            _valueGetter = () => Task.Run(TextGetter).Result;
            _root.clicked += async () => await InstallController.DownloadAndExtractWithDependencies(_mod);
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
            _root.text = _valueGetter();
            var isPopulatingModFile = _mod.Modfile == null;
            if (isPopulatingModFile)
            {
                _root.SetEnabled(false);
                return;
            }

            if (InstalledAddonRepository.Instance.TryGet(_mod.Id, out var manifest))
            {
                var isSameVersion = VersionComparer.IsSameVersion(manifest.Version, _mod.Modfile!.Version);
                _root.SetEnabled(!isSameVersion);
            }
            else
            {
                _root.SetEnabled(true);
            }
        }

        private string TextGetter()
        {
            if (ModManagerUI.UiSystem.ModManagerPanel.CheckForHighestInsteadOfLive)
                return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Download");

            if (!InstalledAddonRepository.Instance.TryGet(_mod.Id, out var manifest)) 
                return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Download");

            if (_mod.Modfile == null)
                return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Download");

            if (VersionComparer.IsSameVersion(_mod.Modfile.Version, manifest.Version))
                return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Download");

            return ModManagerUI.UiSystem.ModManagerPanel.Loc.T("Mods.Update");
        }
    }
}