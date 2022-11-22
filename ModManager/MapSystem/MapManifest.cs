using Modio.Models;
using ModManager.AddonSystem;

namespace ModManager.MapSystem
{
    public class MapManifest : Manifest
    {
        public string MapFileName { get; set; } = null!;

        public MapManifest()
        {
        }

        public MapManifest(Mod mod, File file, string installationRootPath, string mapFleName) 
            : base(mod, file, installationRootPath)
        {
            MapFileName = mapFleName;
        }

        public override Manifest Update(Mod mod, File file)
        {
            MapFileName = $"{mod.NameId!.Trim()}_{file.Id}";
            return base.Update(mod, file);
        }
    }
}