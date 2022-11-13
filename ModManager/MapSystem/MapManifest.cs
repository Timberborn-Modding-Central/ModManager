﻿using Modio.Models;
using ModManager.ModSystem;

namespace ModManager.EnableSystem.Enablers.MapEnablerSystem
{
    public class MapManifest : Manifest
    {
        public string MapFileName { get; set; } = null!;

        public MapManifest()
        {
        }

        public MapManifest(Mod mod, File file, string installationRootPath) : base(mod, file, installationRootPath)
        {
            MapFileName = $"{mod.NameId!.Trim()}_{file.Id}";
        }

        public override Manifest Update(Mod mod, File file)
        {
            MapFileName = $"{mod.NameId!.Trim()}_{file.Id}";
            return base.Update(mod, file);
        }
    }
}