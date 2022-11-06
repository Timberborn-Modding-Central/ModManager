using Modio;

namespace ModManager.ModSystem
{
    public interface IModService
    {
        ModsClient GetMods();

        GameTagsClient GetTags();
    }
}