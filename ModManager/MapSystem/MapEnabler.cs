using System.IO;
using ModManager.EnableSystem;
using ModManager.EnableSystem.Enablers.MapEnablerSystem;
using ModManager.ModSystem;

namespace ModManager.MapSystem
{
    public class MapEnabler : IModEnabler
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

            return true;
        }
    }
}