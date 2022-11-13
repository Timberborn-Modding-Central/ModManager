using Modio;
using Modio.Models;

namespace ModManager.ModSystem
{
    public interface IModService
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