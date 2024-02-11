using Modio.Models;
using ModManager.ModIoSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModCard
{
    public class UninstallButton
    {
        private readonly Button _root;
        private readonly Mod _mod;
        
        public UninstallButton(Button root, Mod mod)
        {
            _root = root;
            _mod = mod;
        }

        public void Initialize()
        {
            _root.clicked += () => InstallController.Uninstall(_mod);
            Refresh();
        }

        public void Refresh()
        {
            _root.visible = IsVisible();
            _root.SetEnabled(IsEnabled());
        }

        private bool IsVisible()
        {
            if (ModHelper.ContainsBepInEx(_mod) || ModHelper.IsModManager(_mod))
            {
                return true;
            }
            
            return _mod.IsInstalled();
        }

        private bool IsEnabled()
        {
            if (ModHelper.ContainsBepInEx(_mod) || ModHelper.IsModManager(_mod))
            {
                return false;
            }

            return true;
        }
    }
}