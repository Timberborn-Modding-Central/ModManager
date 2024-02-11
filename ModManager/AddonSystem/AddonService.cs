using Modio.Models;
using ModManager.AddonEnableSystem;
using ModManager.AddonInstallerSystem;
using ModManager.ModIoSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModManager.VersionSystem;
using File = Modio.Models.File;

namespace ModManager.AddonSystem
{
    public class AddonService : Singleton<AddonService>, IAddonService
    {
        private readonly InstalledAddonRepository _installedAddonRepository = InstalledAddonRepository.Instance;
        private readonly AddonInstallerService _addonInstallerService = AddonInstallerService.Instance;
        private readonly AddonEnablerService _addonEnablerService = AddonEnablerService.Instance;

        private readonly Dictionary<Uri, byte[]> _imageCache = new();

        private readonly HttpClient _httpClient = new();

        public void Install(Mod mod, string zipLocation)
        {
            if (mod.IsInstalled())
            {
                throw new AddonException($"{mod.Name} is already installed. Use method `{nameof(ChangeVersion)}` to change the version of an installed mod.");
            }

            _addonInstallerService.Install(mod, zipLocation);
        }

        public void Uninstall(uint modId)
        {
            if (!_installedAddonRepository.TryGet(modId, out var manifest))
            {
                throw new AddonException($"Cannot uninstall modId: {modId}. Mod is not installed.");
            }

            _addonInstallerService.Uninstall(manifest);
        }

        public void ChangeVersion(Mod mod, File file, string zipLocation)
        {
            if (!mod.IsInstalled())
            {
                throw new AddonException($"Cannot change version of {mod.Name}. Mod is not installed.");
            }
            if (_installedAddonRepository.Get(mod.Id).Version == file.Version)
            {
                throw new AddonException($"{mod.Name} is already installed with version {file.Version}.");
            }

            _addonInstallerService.ChangeVersion(mod, file, zipLocation);
        }

        public void Enable(uint modId)
        {
            if (!_installedAddonRepository.TryGet(modId, out var manifest))
            {
                throw new AddonException($"Cannot enable modId: {modId}. Mod is not installed.");
            }

            _addonEnablerService.Enable(manifest);
        }

        public void Disable(uint modId)
        {
            if (!_installedAddonRepository.TryGet(modId, out var manifest))
            {
                throw new AddonException($"Cannot disable modId: {modId}. Mod is not installed.");
            }

            _addonEnablerService.Disable(manifest);
        }
        
        public IAsyncEnumerable<Dependency> GetDependencies(Mod mod)
        {
            var list = new List<Mod>();

            return GetUniqueDependencies(mod, list);
        }
        
        private static async IAsyncEnumerable<Dependency> GetUniqueDependencies(Mod mod, List<Mod> list)
        {
            list.Add(mod);
            
            var dependencies = ModIoModDependenciesRegistry.Get(mod);

            foreach (var dependency in dependencies)
            {
                yield return dependency;
                
                var dependencyMod = ModIoModRegistry.Get(dependency);
                if (list.Contains(dependencyMod))
                    continue;
                
                await foreach (var dep2 in GetUniqueDependencies(dependencyMod, list))
                {
                    yield return dep2;
                }
            }
        }

        public async Task<(string location, Mod Mod)> Download(Mod mod, File file)
        {
            if (file.IsModInstalled())
            {
                if (!_installedAddonRepository.TryGet(mod.Id, out var manifest))
                {
                    throw new AddonException($"Couldn't find installed mod'd manifest.");
                }
                if (manifest.Version == file.Version)
                {
                    throw new AddonException($"Mod {mod.Name} is already installed with version {file.Version}.");
                }
            }

            mod.Modfile = file;

            Directory.CreateDirectory($"{Paths.ModManager.Temp}");
            var tempZipLocation = Path.Combine(Paths.ModManager.Temp, $"{mod.Id}_{file.Version}.zip");

            await ModIo.Client.Download(ModIoGameInfo.GameId, mod.Id, file.Id, new FileInfo(tempZipLocation));
            (string, Mod) result = new(tempZipLocation, mod);
            return result;
        }

        public async Task<byte[]> GetImage(Uri uri)
        {
            if (_imageCache.TryGetValue(uri, out var imageBytes))
            {
                return imageBytes;
            }

            var byteArray = await _httpClient.GetByteArrayAsync(uri);
            _imageCache[uri] = byteArray;
            return byteArray;
        }
        
        public async Task<File?> TryGetCompatibleVersion(uint modId, bool downloadHighestInsteadOfLive)
        {
            var orderedFiles = await ModIoModFilesRegistry.GetDescAsync(modId);
            var latestCompatibleFile = orderedFiles.FirstOrDefault(file => VersionStatusService.GetVersionStatus(file) == VersionStatus.Compatible);
            if (latestCompatibleFile != null)
            {
                return latestCompatibleFile;
            }

            if (downloadHighestInsteadOfLive)
            {
                var latestUnknownFileOrNull = orderedFiles.FirstOrDefault(file => VersionStatusService.GetVersionStatus(file) == VersionStatus.Unknown);
                return latestUnknownFileOrNull;
            }
            
            return null;
        }
    }
}