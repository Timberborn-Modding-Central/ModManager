using ModManager.AddonSystem;

namespace ModManager.AddonEnableSystem
{
    public interface IAddonEnabler
    {
        bool Enable(Manifest manifest);

        bool Disable(Manifest manifest);
    }
}