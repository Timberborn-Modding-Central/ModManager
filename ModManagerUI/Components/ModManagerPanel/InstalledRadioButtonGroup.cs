using System;
using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using ModManager.AddonSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class InstalledRadioButtonGroup : CustomRadioButtonGroup, IFilterIdsProvider
    {
        public InstalledRadioButtonGroup(VisualElement root, TagOption tagOption) : base(root, tagOption)
        {
        }

        public List<uint> ProvideFilterIds(out bool isNotList)
        {
            isNotList = false;
            if (!HasTagSelected() || !Enum.TryParse(GetActiveTag(), out InstalledOptions installedOptions))
                return new List<uint>();
            switch (installedOptions)
            {
                case InstalledOptions.Installed:
                    var installedModNames = InstalledAddonRepository.Instance.All().Select(manifest => manifest.ModId).ToList();
                    return installedModNames;
                case InstalledOptions.Uninstalled:
                    isNotList = true;
                    var uninstalledModNames = InstalledAddonRepository.Instance.All().Select(manifest => manifest.ModId).ToList();
                    return uninstalledModNames;
                case InstalledOptions.UpdateAvailable:
                    if (UpdateableModRegistry.UpdateAvailable == null)
                        return new List<uint>();
                    var updateAvailableNames = UpdateableModRegistry.UpdateAvailable.Select(pair => pair.Value.ModId).ToList();
                    return updateAvailableNames;
                default:
                    throw new ArgumentOutOfRangeException($"Value '{installedOptions}' not a valid {nameof(InstalledOptions)}.");
            }
        }
    }
}