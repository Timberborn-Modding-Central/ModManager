using Modio.Models;
using ModManager.ExtractorSystem;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ModManager.MapSystem
{
    public class MapExtractor : IAddonExtractor
    {
        public bool Extract(string addonZipLocation, Mod modInfo, out string extractLocation, bool overWrite = true)
        {
            extractLocation = "";
            if (!modInfo.Tags.Any(x => x.Name == "Map"))
            {
                return false;
            }
            using var zipFile = ZipFile.OpenRead(addonZipLocation);
            var timberFile = zipFile.Entries
                                    .Where(x => x.Name.Contains(".timber"))
                                    .SingleOrDefault() ?? throw new MapException("Map zip does not contain an entry for a .timber file");

            timberFile.ExtractToFile(Path.Combine(Paths.Maps, timberFile.Name));

            extractLocation = Paths.Maps;
            System.IO.File.Delete(addonZipLocation);

            return true;
        }
    }
}
