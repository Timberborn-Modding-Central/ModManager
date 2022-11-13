using Modio;
using Modio.Models;
using ModManager.AddonEnableSystem;
using ModManager.AddonInstallerSystem;
using ModManager.ModIoSystem;
using ModManager.ModSystem;
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

        public void Install(Mod mod, File file)
        {
            if(_installedAddonRepository.Has(mod.Id))
            {
                throw new AddonException($"{mod.Name} is already installed. Use method `{nameof(ChangeVersion)}` to change the version of an installed mod.");
            }

            _addonInstallerService.Install(mod, file);
        }

        public void Uninstall(uint modId)
        {
            if (! _installedAddonRepository.TryGet(modId, out Manifest manifest))
            {
                throw new AddonException($"Cannot uninstall modId: {modId}. Mod is not installed.");
            }

            _addonInstallerService.Uninstall(manifest);
        }

        public void ChangeVersion(Mod mod, File file)
        {
            if (! _installedAddonRepository.Has(mod.Id))
            {
                throw new AddonException($"Cannot change version of {mod.Name}. Mod is not installed.");
            }

            _addonInstallerService.ChangeVersion(mod, file);
        }

        public void Enable(uint modId)
        {
            if (! _installedAddonRepository.TryGet(modId, out Manifest? manifest))
            {
                throw new AddonException($"Cannot enable modId: {modId}. Mod is not installed.");
            }

            _addonEnablerService.Enable(manifest);
        }

        public void Disable(uint modId)
        {
            if (! _installedAddonRepository.TryGet(modId, out Manifest manifest))
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
    }
}