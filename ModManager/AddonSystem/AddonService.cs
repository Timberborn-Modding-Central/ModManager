using Modio;
using Modio.Models;
using ModManager.AddonEnableSystem;
using ModManager.AddonInstallerSystem;
using ModManager.ModIoSystem;
using ModManager.ModSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using File = Modio.Models.File;

namespace ModManager.AddonSystem
{
    public class AddonService : Singleton<AddonService>, IAddonService
    {
        private readonly InstalledAddonRepository _installedAddonRepository;

        private readonly AddonInstallerService _addonInstallerService;

        private readonly AddonEnablerService _addonEnablerService;

        public AddonService()
        {
            _addonEnablerService = AddonEnablerService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
            _addonInstallerService = AddonInstallerService.Instance;
        }

        public void Install(Mod mod, string zipLocation)
        {
            if (_installedAddonRepository.Has(mod.Id))
            {
                throw new AddonException($"{mod.Name} is already installed. Use method `{nameof(ChangeVersion)}` to change the version of an installed mod.");
            }

            _addonInstallerService.Install(mod, zipLocation);
        }

        public void Uninstall(uint modId)
        {
            if (!_installedAddonRepository.TryGet(modId, out Manifest manifest))
            {
                throw new AddonException($"Cannot uninstall modId: {modId}. Mod is not installed.");
            }

            _addonInstallerService.Uninstall(manifest);
        }

        public void ChangeVersion(Mod mod, File file, string zipLocation)
        {
            if (!_installedAddonRepository.Has(mod.Id))
            {
                throw new AddonException($"Cannot change version of {mod.Name}. Mod is not installed.");
            }

            _addonInstallerService.ChangeVersion(mod, file, zipLocation);
        }

        public void Enable(uint modId)
        {
            if (!_installedAddonRepository.TryGet(modId, out Manifest? manifest))
            {
                throw new AddonException($"Cannot enable modId: {modId}. Mod is not installed.");
            }

            _addonEnablerService.Enable(manifest);
        }

        public void Disable(uint modId)
        {
            if (!_installedAddonRepository.TryGet(modId, out Manifest manifest))
            {
                throw new AddonException($"Cannot disable modId: {modId}. Mod is not installed.");
            }

            _addonEnablerService.Disable(manifest);
        }

        public ModsClient GetMods()
        {
            return ModIo.Client.Games[ModIoGameInfo.GameId].Mods;
        }

        public GameTagsClient GetTags()
        {
            return ModIo.Client.Games[ModIoGameInfo.GameId].Tags;
        }

        public async Task<(string location, Mod Mod)> DownloadLatest(uint modId)
        {
            //if(_installedAddonRepository.All().Any(x => x.Version))
            if (ModIo.Client.Games[ModIoGameInfo.GameId].Mods[modId].IsInstalled())
            {
                throw new AddonException($"Mod with id {modId} is already installed.");
            }
            var mod = await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[modId].Get();

            Directory.CreateDirectory($"{Paths.ModManager.Temp}");
            string tempZipLocation = Path.Combine(Paths.ModManager.Temp, $"{modId}_{mod.Modfile.Id}.zip");

            await ModIo.Client.Download(ModIoGameInfo.GameId,
                                        modId,
                                        new FileInfo(tempZipLocation));
            (string, Mod) result = new(tempZipLocation, mod);
            return result;
        }

        private async IAsyncEnumerable<Dependency> GetDependencies(uint modid)
        {
            var deps = await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[modid].Dependencies.Get();


            foreach (var dep in deps)
            {
                yield return dep;
                await foreach (var dep2 in GetDependencies(dep.ModId))
                {
                    yield return dep2;
                }
            }
        }

        public async IAsyncEnumerable<(string location, Mod Mod)> DownloadDependencies(Mod mod)
        {
            await foreach (var dep in GetDependencies(mod.Id))
            {
                (string location, Mod Mod) returnvalue = new();
                try
                {
                    returnvalue = await DownloadLatest(dep.ModId);
                }
                catch (AddonException ex)
                {
                    continue;
                }
                yield return returnvalue;
            }
        }

        public async Task<(string location, Mod Mod)> Download(uint modId, uint fileId)
        {
            File file = new();
            if (ModIo.Client.Games[ModIoGameInfo.GameId].Mods[modId].IsInstalled())
            {
                if (!_installedAddonRepository.TryGet(modId, out Manifest manifest))
                {
                    throw new AddonException($"Couldn't find installed mod'd manifest.");
                }
                file = await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[modId].Files[fileId].Get();
                if (manifest.Version == file.Version)
                {
                    throw new AddonException($"Mod with id {modId} is already installed with version {file.Version}.");
                }
            }

            var mod = await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[modId].Get();
            mod.Modfile = file;

            Directory.CreateDirectory($"{Paths.ModManager.Temp}");
            string tempZipLocation = Path.Combine(Paths.ModManager.Temp, $"{modId}_{fileId}.zip");

            await ModIo.Client.Download(ModIoGameInfo.GameId,
                                   modId,
                                   fileId,
                                   new FileInfo(tempZipLocation));
            (string, Mod) result = new(tempZipLocation, mod);
            return result;
        }

        public async Task<byte[]> GetImage(Uri uri)
        {
            using var client = new HttpClient();
            var byteArray = await client.GetByteArrayAsync(uri);

            return byteArray;
        }
    }
}