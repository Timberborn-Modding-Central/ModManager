using Modio.Models;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManager.VersionSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModFullInfo
{
    public class DownloadButton
    {
        private readonly Button _root;
        private readonly Mod _mod;
        private readonly ModFullInfoController _modFullInfoController;

        private DownloadButton(Button root, Mod mod, ModFullInfoController modFullInfoController)
        {
            _root = root;
            _mod = mod;
            _modFullInfoController = modFullInfoController;
        }

        public static DownloadButton Create(Button root, Mod mod, ModFullInfoController modFullInfoController)
        {
            var downloadButton = new DownloadButton(root, mod, modFullInfoController);
            root.clicked += () =>
            {
                root.SetEnabled(false);
                downloadButton.Download();
            };
            downloadButton.Refresh();
            return downloadButton;
        }

        private async void Download()
        {
            await InstallController.DownloadAndExtract(_mod, _modFullInfoController.CurrentFile);
            Refresh();
        }

        public void Refresh()
        {
            _root.text = TextGetter();
            if (_mod.Modfile == null)
            {
                _root.SetEnabled(false);
                return;
            }

            if (!_mod.IsInstalled())
            {
                _root.SetEnabled(true);
                return;
            }

            if (_modFullInfoController.CurrentFile == null)
            {
                _root.SetEnabled(true);
                return;
            }
            
            _root.SetEnabled(!VersionComparer.IsSameVersion(_modFullInfoController.CurrentFile.Version, _mod.Modfile.Version));
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