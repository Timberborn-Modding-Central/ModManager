using System;
using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using ModManager.AddonSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class EnabledRadioButtonGroup : CustomRadioButtonGroup, IFilterIdsProvider
    {
        public EnabledRadioButtonGroup(VisualElement root, TagOption tagOption) : base(root, tagOption)
        {
        }

        public List<uint> ProvideFilterIds(out bool isNotList)
        {
            isNotList = false;
            if (!HasTagSelected() || !Enum.TryParse(GetActiveTag(), out EnabledOptions enabledOptions))
                return new List<uint>();
            switch (enabledOptions)
            {
                case EnabledOptions.Enabled:
                    var enabledModIds = InstalledAddonRepository.Instance.All().Where(x => x.Enabled).Select(x => x.ModId).ToList();
                    return enabledModIds;
                case EnabledOptions.NotEnabled:
                    var notEnabledModIds = InstalledAddonRepository.Instance.All().Where(x => !x.Enabled).Select(x => x.ModId).ToList();
                    return notEnabledModIds;
                default:
                    throw new ArgumentOutOfRangeException($"Value '{enabledOptions}' not a valid {nameof(EnabledOptions)}.");
            }
        }
    }
}