using System;
using Modio.Models;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModCard
{
    public class EnabledToggle
    {
        private readonly IAddonService _addonService = AddonService.Instance;
        
        private readonly Toggle _root;
        private readonly Mod _mod;
        
        private Func<bool> _valueGetter = () => false;
        private Func<bool> _visibilityGetter = () => false;
        private Action _initializer = delegate {  };

        private bool _initialized;
        
        public EnabledToggle(Toggle root, Mod mod)
        {
            _root = root;
            _mod = mod;
        }

        public void TryInitializing()
        {
            if (_mod.IsInstalled() || ModHelper.IsModManager(_mod))
            {
                _root.RegisterValueChangedCallback(changeEvent => OnToggleValueChanged(changeEvent, _mod));
                if (ModHelper.ContainsBepInEx(_mod) || ModHelper.IsModManager(_mod))
                {
                    _valueGetter = () => true;
                    _visibilityGetter = () => true;
                }
                else
                {
                    _valueGetter = _mod.IsEnabled;
                    _visibilityGetter = _mod.IsInstalled;
                }
                _initialized = true;
                Refresh();
                return;
            }
            
            _initializer = TryInitializing;
        }

        public void Refresh()
        {
            if (!_initialized) 
                _initializer();
            _root.SetValueWithoutNotify(_valueGetter());
            _root.visible = _visibilityGetter();
        }
        
        private void OnToggleValueChanged(ChangeEvent<bool> changeEvent, Mod mod)
        {
            _root.SetValueWithoutNotify(EnableController.AllowedToChangeState(mod) ? changeEvent.newValue : _valueGetter());
            EnableController.ChangeState(mod, changeEvent.newValue);
            Refresh();
        }
    }
}