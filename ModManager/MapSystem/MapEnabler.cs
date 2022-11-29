using System.IO;
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

            string enabledFilePath = Path.Combine(Paths.Maps, mapManifest.MapFileName + Names.Extensions.TimberbornMap);

            if (! File.Exists(enabledFilePath + Names.Extensions.Disabled))
            {
                return true;
            }

            File.Move(enabledFilePath + Names.Extensions.Disabled, enabledFilePath);
            manifest.Enabled = true;

            return true;
        }

        public bool Disable(Manifest manifest)
        {
            if (manifest is not MapManifest mapManifest)
            {
                return false;
            }

            string mapFilePath = Path.Combine(Paths.Maps, mapManifest.MapFileName + Names.Extensions.TimberbornMap);

            if (! File.Exists(mapFilePath))
            {
                return true;
            }

            File.Move(mapFilePath, mapFilePath + Names.Extensions.Disabled);
            manifest.Enabled = false;

            return true;
        }
    }
}