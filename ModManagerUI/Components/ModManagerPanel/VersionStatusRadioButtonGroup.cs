using System;
using Modio.Models;
using ModManager.VersionSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class VersionStatusRadioButtonGroup : CustomRadioButtonGroup
    {
        public VersionStatusRadioButtonGroup(VisualElement root, TagOption tagOption) : base(root, tagOption)
        {
        }

        protected override void OnValueChanged()
        {
            if (!HasTagSelected() || !Enum.TryParse(GetActiveTag(), out VersionStatus versionStatus))
            {
                FilterController.VersionStatusFilter = null;
                base.OnValueChanged();
                return;
            }
            FilterController.VersionStatusFilter = versionStatus;
            base.OnValueChanged();
        }
    }
}