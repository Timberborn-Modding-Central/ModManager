using System.Collections.Generic;
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

            List<string> enabledFilePaths = mapManifest.MapFileNames
                                                       .Select(mapfilename => Path.Combine(Paths.Maps, mapfilename + Names.Extensions.TimberbornMap))
                                                       .ToList();

            foreach (string enabledFilePath in enabledFilePaths)
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

            List<string> mapFilePaths = mapManifest.MapFileNames
                                                   .Select(mapfilename => Path.Combine(Paths.Maps, mapfilename + Names.Extensions.TimberbornMap))
                                                   .ToList();

            foreach (string mapFilePath in mapFilePaths)
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