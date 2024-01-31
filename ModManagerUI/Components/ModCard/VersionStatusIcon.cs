using Modio.Models;
using ModManager.VersionSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace ModManagerUI.Components.ModCard
{
    public class VersionStatusIcon
    {
        private readonly ITooltipRegistrar _tooltipRegistrar;
        private readonly ILoc _loc;
        
        private readonly VisualElement _root;
        private readonly Mod _mod;

        private Image? _statusIconUnknown;
        private Image? _statusIconCompatible;
        private Image? _statusIconIncompatible;
        
        public VersionStatusIcon(VisualElement root, Mod mod)
        {
            _tooltipRegistrar = UiSystem.ModManagerPanel.TooltipRegistrar;
            _loc = UiSystem.ModManagerPanel.Loc;
            _root = root;
            _mod = mod;
        }

        public void Initialize()
        {
            // _root.ToggleDisplayStyle(!ModManagerUI.ModsBox.IsExperimental);
            
            _statusIconUnknown = _root.Q<Image>("StatusIconUnknown");
            _statusIconCompatible = _root.Q<Image>("StatusIconCompatible");
            _statusIconIncompatible = _root.Q<Image>("StatusIconIncompatible");
            
            _tooltipRegistrar.Register(_statusIconUnknown, _loc.T("Mods.VersionUnknown"));
            _tooltipRegistrar.Register(_statusIconCompatible, _loc.T("Mods.VersionCompatible"));
            _tooltipRegistrar.Register(_statusIconIncompatible, _loc.T("Mods.VersionIncompatible"));
            
            Refresh();
        }

        public void Refresh()
        {
            UpdateTask();
        }

        private void UpdateTask()
        {
            var  versionStatus = VersionStatusService.GetVersionStatus(_mod.Modfile);
            
            _statusIconUnknown.ToggleDisplayStyle(versionStatus == VersionStatus.Unknown);
            _statusIconCompatible.ToggleDisplayStyle(versionStatus == VersionStatus.Compatible);
            _statusIconIncompatible.ToggleDisplayStyle(versionStatus == VersionStatus.Incompatible);
        }
    }
}