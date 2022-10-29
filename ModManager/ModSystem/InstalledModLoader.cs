using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ModManagerWrapper.ModSystem
{
    public class InstalledModLoader
    {
        private static readonly string Manifest = "manifest.json";

        public IEnumerable<Manifest> GetAllInstalledManifests()
        {
            string[] files = Directory.GetFiles(Paths.Mods, Manifest, SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string manifestJson = File.ReadAllText(file);

                yield return JsonConvert.DeserializeObject<Manifest>(manifestJson)!;
            }
        }
    }
}