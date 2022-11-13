using ModManager.AddonSystem;
using ModManager.ModSystem;

namespace ModManager.AddonEnableSystem
{
    public interface IAddonEnabler
    {
        bool Enable(Manifest manifest);

        bool Disable(Manifest manifest);
    }
}