using System.Collections.Generic;
using ModManager.AddonSystem;

namespace ModManager.AddonEnableSystem
{
    public class AddonEnablerService : Singleton<AddonEnablerService>
    {
        private readonly AddonEnablerRegistry _addonEnablerRegistry = AddonEnablerRegistry.Instance;

        public static readonly IEnumerable<string> IgnoreExtensions = new[]
        {
            Names.Extensions.Disabled,
            Names.Extensions.Remove
        };

        public void Enable(Manifest manifest)
        {
            foreach (var enabler in _addonEnablerRegistry.GetAddonEnablers())
            {
                if (enabler.Enable(manifest))
                {
                    return;
                }
            }

            throw new AddonEnablerException($"{manifest.ModName} could not be enabled by any mod enabler");
        }

        public void Disable(Manifest manifest)
        {
            foreach (var enabler in _addonEnablerRegistry.GetAddonEnablers())
            {
                if (enabler.Disable(manifest))
                {
                    return;
                }
            }

            throw new AddonEnablerException($"{manifest.ModName} could not be disabled by any mod enabler");
        }
    }
}