using Modio;
using Modio.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        Task<(string location, Mod Mod)> DownloadLatest(uint modId);

        Task<List<(string location, Mod Mod)>> DownloadDependencies(Mod mod);

        Task<(string location, Mod Mod)> Download(uint modId, uint fileId);

        Task<byte[]> GetImage(Uri uri);
    }
}