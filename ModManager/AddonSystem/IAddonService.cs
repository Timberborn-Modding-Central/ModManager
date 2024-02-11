using Modio.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModManager.AddonSystem
{
    public interface IAddonService
    {
        void Install(Mod mod, string zipLocation);

        void Uninstall(uint modId);

        void ChangeVersion(Mod mod, File file, string zipLocation);

        void Enable(uint modId);

        void Disable(uint modId);

        IAsyncEnumerable<Dependency> GetDependencies(Mod mod);

        Task<(string location, Mod Mod)> Download(Mod mod, File file);

        Task<byte[]> GetImage(Uri uri);

        Task<File?> TryGetCompatibleVersion(uint modId, bool downloadHighestInsteadOfLive);
    }
}