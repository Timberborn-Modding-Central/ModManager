using ModManager;
using ModManager.StartupSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModManagerUI
{
    public class UIPaths : Singleton<Paths>, ILoadable
    {
        public void Load(ModManagerStartupOptions startupOptions)
        {
            EnsurePathExists(ModManagerUI.Assets);
            EnsurePathExists(ModManagerUI.Lang);
        }

        private static void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static class ModManagerUI
        {
            public static string Assets { get; set; } = Path.Combine(Paths.ModManagerRoot, "assets");

            public static string Lang { get; set; } = Path.Combine(Paths.ModManagerRoot, "lang");
        }
    }
}
