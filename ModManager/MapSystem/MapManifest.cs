using Modio.Models;
using ModManager.AddonSystem;
using System.Collections.Generic;

namespace ModManager.MapSystem
{
    public class MapManifest : Manifest
    {
        public new const string FileName = Manifest.FileName;

        public List<string> MapFileNames { get; set; } = null!;

        public MapManifest()
        {
        }

        public MapManifest(Mod mod, File file, string installationRootPath, List<string> mapFleNames) 
            : base(mod, file, installationRootPath)
        {
            MapFileNames = mapFleNames;
        }

        public override Manifest Update(Mod mod, File file)
        {
            //MapFileNames = $"{mod.NameId!.Trim()}_{file.Id}";
            return base.Update(mod, file);
        }
    }
}