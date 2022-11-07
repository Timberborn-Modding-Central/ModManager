using ModManager.ModSystem;

namespace ModManager.EnableSystem
{
    public interface IModEnabler
    {
        bool Enable(Manifest manifest);

        bool Disable(Manifest manifest);
    }
}