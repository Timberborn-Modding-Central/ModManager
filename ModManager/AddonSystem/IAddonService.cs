using Modio;
using Modio.Models;

namespace ModManager.AddonSystem
{
    public interface IAddonService
    {
        void Install(Mod mod, File file);

        void Uninstall(uint modId);

        void ChangeVersion(Mod mod, File file);

        void Enable(uint modId);

        void Disable(uint modId);

        ModsClient GetMods();

        GameTagsClient GetTags();
    }
}