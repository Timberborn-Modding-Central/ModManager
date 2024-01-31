using System;
using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using ModManager.VersionSystem;
using ModManagerUI.UiSystem;
using Timberborn.DropdownSystem;
using UnityEngine;

namespace ModManagerUI.Components.ModFullInfo
{
    public class VersionDropdownProvider : IExtendedDropdownProvider
    {
        private readonly ModFullInfoController _infoController;
        private readonly List<File> _versions;
        
        public VersionDropdownProvider(
            ModFullInfoController infoController,
            List<File> versions)
        {
            _infoController = infoController;
            _versions = versions;
        }

        public IReadOnlyList<string> Items => _versions.Select(x => x.Version ?? "").ToList();

        public string GetValue()
        {
            return _infoController.CurrentFile!.Version ?? "";
        }

        public void SetValue(string value)
        {
            var currFile = _versions.SingleOrDefault(x => (x.Version ?? "") == value);
            _infoController.CurrentFile = currFile;
        }

        public string FormatDisplayText(string value)
        {
            return value;
        }

        public Sprite GetIcon(string value)
        {
            var versionStatus = VersionStatusService.GetVersionStatus(_infoController.CurrentFile!.ModId, value);
            switch (versionStatus)
            {
                case VersionStatus.Unknown:
                    return StatusIconLoader.Instance.UnknownSprite;
                case VersionStatus.Compatible:
                    return StatusIconLoader.Instance.CompatibleSprite;
                case VersionStatus.Incompatible:
                    return StatusIconLoader.Instance.IncompatibleSprite;
                default:
                    throw new ArgumentOutOfRangeException(versionStatus.ToString());
            }
        }
    }
}
