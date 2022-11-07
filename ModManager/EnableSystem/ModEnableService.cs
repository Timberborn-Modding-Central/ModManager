using System;
using System.Collections.Generic;
using ModManager.EnableSystem.Enablers;
using ModManager.ModSystem;
using ModManager.SingletonInstanceSystem;

namespace ModManager.EnableSystem
{
    public class ModEnableService : Singleton<ModEnableService>
    {
        public static readonly string DisabledExtension = ".disabled";

        public static readonly IEnumerable<string> IgnoreExtensions = new[]
        {
            DisabledExtension,
            ".delete"
        };

        private readonly List<IModEnabler> _enablers = new()
        {
            new GeneralEnabler()
        };

        public void Enable(Manifest manifest)
        {
            foreach (IModEnabler enabler in _enablers)
            {
                if (enabler.Enable(manifest))
                {
                    return;
                }
            }

            throw new Exception($"{manifest.ModName} could not be enabled by any mod enabler");
        }

        public void Disable(Manifest manifest)
        {
            foreach (IModEnabler enabler in _enablers)
            {
                if (enabler.Disable(manifest))
                {
                    return;
                }
            }

            throw new Exception($"{manifest.ModName} could not be disabled by any mod enabler");
        }

        public void AddModEnabler(IModEnabler installer)
        {
            _enablers.Insert(0, installer);
        }
    }
}