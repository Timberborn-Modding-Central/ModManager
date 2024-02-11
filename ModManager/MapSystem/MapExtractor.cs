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
            using (var zipFile = ZipFile.OpenRead(addonZipLocation))
            {
                var timberFiles = zipFile.Entries
                                         .Where(x => x.Name.Contains(".timber"))
                                         .ToList();

                if (timberFiles.Count() == 0)
                {
                    throw new MapException("Map zip does not contain an entry for a .timber file");
                }

                foreach (var timberFile in timberFiles)
                {
                    var filename = timberFile.Name.Replace(Names.Extensions.TimberbornMap, "");
                    var files = Directory.GetFiles(Paths.Maps, filename);
                    if (files.Length > 0)
                    {
                        filename += $"_{files.Length + 1}";
                    }

                    timberFile.ExtractToFile(Path.Combine(Paths.Maps, timberFile.Name), overWrite);
                }
            }

            extractLocation = Paths.Maps;
            System.IO.File.Delete(addonZipLocation);

            return true;
        }
    }
}
