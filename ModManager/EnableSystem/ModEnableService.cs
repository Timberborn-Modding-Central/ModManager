using System.Collections.Generic;
using ModManager.MapSystem;
using ModManager.ModSystem;

namespace ModManager.EnableSystem
{
    public class ModEnableService : Singleton<ModEnableService>
    {
        public static readonly IEnumerable<string> IgnoreExtensions = new[]
        {
            Names.Extensions.Disabled,
            Names.Extensions.Remove
        };

        private readonly List<IModEnabler> _enablers = new()
        {
            new MapEnabler(),
            new ModEnabler()
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

            throw new EnableServiceException($"{manifest.ModName} could not be enabled by any mod enabler");
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

            throw new EnableServiceException($"{manifest.ModName} could not be disabled by any mod enabler");
        }

        public void AddModEnabler(IModEnabler installer)
        {
            _enablers.Insert(0, installer);
        }
    }
}