using ModManager;
using ModManager.StartupSystem;
using System.IO;

namespace ModManagerUI
{
    public class UIPaths : Singleton<UIPaths> , ILoadable
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
