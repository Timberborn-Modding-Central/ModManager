using System.IO;
using System.Linq;
using ModManager.AddonEnableSystem;
using ModManager.AddonSystem;

namespace ModManager.MapSystem
{
    public class MapEnabler : IAddonEnabler
    {
        public bool Enable(Manifest manifest)
        {
            if (manifest is not MapManifest mapManifest)
            {
                return false;
            }

            var enabledFilePaths = mapManifest.MapFileNames.Select(mapFileName => Path.Combine(Paths.Maps, mapFileName + Names.Extensions.TimberbornMap)).ToList();

            foreach (var enabledFilePath in enabledFilePaths)
            {
                if (!File.Exists(enabledFilePath + Names.Extensions.Disabled))
                {
                    continue;
                }

                File.Move(enabledFilePath + Names.Extensions.Disabled, enabledFilePath);
                manifest.Enabled = true;
            }
            return true;
        }

        public bool Disable(Manifest manifest)
        {
            if (manifest is not MapManifest mapManifest)
            {
                return false;
            }

            var mapFilePaths = mapManifest.MapFileNames.Select(mapFileName => Path.Combine(Paths.Maps, mapFileName + Names.Extensions.TimberbornMap)).ToList();

            foreach (var mapFilePath in mapFilePaths)
            {
                if (!File.Exists(mapFilePath))
                {
                    continue;
                }

                File.Move(mapFilePath, mapFilePath + Names.Extensions.Disabled);
                manifest.Enabled = false;
            }

            return true;
        }
    }
}