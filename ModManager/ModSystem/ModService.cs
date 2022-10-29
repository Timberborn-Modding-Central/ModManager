using System.Collections.Generic;
using System.IO;
using Modio;
using Modio.Models;
using ModManagerWrapper.ModIoSystem;
using Newtonsoft.Json;
using File = System.IO.File;

namespace ModManagerWrapper.ModSystem
{
    public class ModService
    {
        private static readonly string InstalledModsFilePath = Path.Combine(Paths.ModManager.User, "installedMods.json");

        private readonly InstalledModRepository _installedModRepository;

        private readonly Client _modIo;

        public ModService()
        {
            Dictionary<uint, Manifest> installedMods = new();
            if (File.Exists(InstalledModsFilePath))
            {
                JsonConvert.DeserializeObject<List<Manifest>>(InstalledModsFilePath);

                // installedMods = JsonConvert.DeserializeObject<Dictionary<uint, InstalledMod>>(InstalledModsFilePath)!;
            }
            // _installedModRepository = new InstalledModRepository(installedMods);
            _modIo = ModIo.Instance.Client;
        }

        public void Subscribe(Mod mod)
        {
            _installedModRepository.Add(mod);
        }

        public void UnSubscribe()
        {

        }

        public void Enable()
        {

        }

        public void Disable()
        {

        }
    }
}