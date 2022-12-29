using Modio.Models;

namespace ModManager.ExtractorSystem
{
    public interface IAddonExtractor
    {
        bool Extract(string addonZipLocation, Mod modInfo, out string extractLocation, bool overWrite = true);
    }
}
