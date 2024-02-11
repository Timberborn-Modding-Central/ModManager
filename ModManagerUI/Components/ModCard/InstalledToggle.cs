using System;
using Modio.Models;
using ModManager.AddonSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModCard
{
    public class InstalledToggle
    {
        private readonly Toggle _root;
        
        private Func<bool> _valueGetter = () => false;
        
        public InstalledToggle(Toggle root)
        {
            _root = root;
        }

        public void Initialize(Mod mod)
        {
            _root.RegisterValueChangedCallback(_ => OnToggleValueChanged());
            _valueGetter = () => InstalledAddonRepository.Instance.Has(mod.Id) || ModHelper.IsModManager(mod);
            Refresh();
        }
        
        public void Refresh()
        {
            _root.SetValueWithoutNotify(_valueGetter());
        }
        
        private void OnToggleValueChanged()
        {
            // HACK: using SetEnabled(false) greys the Toggle and dunno how to fix that ;_;
            _root.SetValueWithoutNotify(_valueGetter());
        }
    }
}