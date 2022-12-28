using Modio.Models;
using ModManager.ExtractorSystem;
using System;
using System.IO.Compression;
using System.IO;
using System.Linq;

namespace ModManager.BepInExSystem
{
    public class BepInExExtractor : IAddonExtractor
    {
        private const string _bepInExPackName = "BepInExPack";

        // TODO: Add actual extract logic from bepinex. 
        //       Atm bepinex must be installed already for mod manager to work
        //       and updated to the pack in unlikely
        public bool Extract(string addonZipLocation, Mod modInfo, out string extractLocation, bool overWrite = true)
        {
            extractLocation = "";
            if (!modInfo.Tags.Any(x => x.Name == "Mod") ||
                modInfo.Name != _bepInExPackName)
            {
                return false;
            }

            extractLocation = Path.Combine(Paths.GameRoot, "BepInEx", "plugins", $"{modInfo.NameId}_{modInfo.Id}_{modInfo.Modfile.Version}");
            if (!Directory.Exists(extractLocation))
            {
                Directory.CreateDirectory(extractLocation);
            }

            System.IO.File.Delete(addonZipLocation);

            return true;
        }
    }
}
