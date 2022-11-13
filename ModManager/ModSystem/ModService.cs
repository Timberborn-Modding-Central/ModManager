using System;
using Modio;
using Modio.Models;
using ModManager.EnableSystem;
using ModManager.InstallerSystem;
using ModManager.ModIoSystem;
using File = Modio.Models.File;

namespace ModManager.ModSystem
{
    public class ModService : Singleton<ModService>, IModService
    {
        private readonly InstalledModRepository _installedModRepository;

        private readonly ModInstallerService _modInstallerService;

        private readonly ModEnableService _modEnableService;

        public ModService()
        {
            _modEnableService = ModEnableService.Instance;
            _installedModRepository = InstalledModRepository.Instance;
            _modInstallerService = ModInstallerService.Instance;
        }

        public void Install(Mod mod, File file)
        {
            if(_installedModRepository.Has(mod.Id))
            {
                throw new Exception($"{mod.Name} is already installed. Use method `ChangeVersion` to change the version of an installed mod.");
            }

            _modInstallerService.Install(mod, file);
        }

        public void Uninstall(uint modId)
        {
            if (! _installedModRepository.TryGet(modId, out Manifest manifest))
            {
                throw new Exception($"Cannot uninstall modId: {modId}. Mod is not installed.");
            }

            _modInstallerService.Uninstall(manifest);
        }

        public void ChangeVersion(Mod mod, File file)
        {
            if (! _installedModRepository.Has(mod.Id))
            {
                throw new Exception($"Cannot change version of {mod.Name}. Mod is not installed.");
            }

            _modInstallerService.ChangeVersion(mod, file);
        }

        public void Enable(uint modId)
        {
            if (! _installedModRepository.TryGet(modId, out Manifest? manifest))
            {
                throw new Exception($"Cannot enable modId: {modId}. Mod is not installed.");
            }

            _modEnableService.Enable(manifest);
        }

        public void Disable(uint modId)
        {
            if (! _installedModRepository.TryGet(modId, out Manifest manifest))
            {
                throw new Exception($"Cannot disable modId: {modId}. Mod is not installed.");
            }

            _modEnableService.Disable(manifest);
        }

        public ModsClient GetMods()
        {
            return ModIo.Client.Games[ModIoGameInfo.GameId].Mods;
        }

        public GameTagsClient GetTags()
        {
            return ModIo.Client.Games[ModIoGameInfo.GameId].Tags;
        }
    }
}