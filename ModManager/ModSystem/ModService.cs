using System;
using System.IO;
using Modio;
using Modio.Models;
using ModManager.EnableSystem;
using ModManager.InstallerSystem;
using ModManager.ModIoSystem;
using ModManager.PersistenceSystem;
using ModManager.SingletonInstanceSystem;
using File = Modio.Models.File;

namespace ModManager.ModSystem
{
    public class ModService : Singleton<ModService>, IModService
    {
        private readonly InstalledModRepository _installedModRepository;

        private readonly InstallerService _installerService;

        private readonly PersistenceService _persistenceService;

        private readonly ModEnabler _modEnabler;

        public ModService()
        {
            _modEnabler = ModEnabler.Instance;
            _installedModRepository = InstalledModRepository.Instance;
            _installerService = InstallerService.Instance;
            _persistenceService = PersistenceService.Instance;
        }

        public void Install(Mod mod, File file)
        {
            if(_installedModRepository.Has(mod))
            {
                throw new Exception($"{mod.Name} is already installed. Use method `ChangeVersion` to change the version of an installed mod.");
            }

            // string installationPath = _installerService.Install(mod, file);

            string installationPath = @"C:\Program Files (x86)\Steam\steamapps\common\Timberborn\BepInEx\plugins\CategoryButton\plugins";

            Manifest installedModManifest = _installedModRepository.Add(mod, file, installationPath);

            _persistenceService.SaveObject(installedModManifest, Path.Combine(installationPath, Manifest.FileName));
        }

        public void Uninstall(Mod mod)
        {
            if (! _installedModRepository.TryGet(mod, out Manifest manifest))
            {
                throw new Exception($"Cannot uninstall {mod.Name}. Mod is not installed.");
            }

            _installerService.Uninstall(mod, manifest);

            _installedModRepository.Remove(mod);
        }

        public void ChangeVersion(Mod mod, File file)
        {
            if (! _installedModRepository.TryGet(mod, out Manifest manifest))
            {
                throw new Exception($"Cannot change version of {mod.Name}. Mod is not installed.");
            }

            string installationPath = _installerService.ChangeVersion(mod, file);

            manifest.Update(mod, file);

            _persistenceService.SaveObject(manifest, installationPath);
        }

        public void Enable(Mod mod)
        {
            if (! _installedModRepository.TryGet(mod, out Manifest manifest))
            {
                throw new Exception($"Cannot enable {mod.Name}. Mod is not installed.");
            }

            _modEnabler.Enable(manifest);
        }

        public void Disable(Mod mod)
        {
            if (! _installedModRepository.TryGet(mod, out Manifest manifest))
            {
                throw new Exception($"Cannot disable {mod.Name}. Mod is not installed.");
            }

            _modEnabler.Disable(manifest);
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