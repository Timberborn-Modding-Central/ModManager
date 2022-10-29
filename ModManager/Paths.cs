using System.IO;
using ModManagerWrapper.StartupSystem;

namespace ModManagerWrapper
{
    public class Paths : ILoadable
    {
        public void Load(ModManagerStartupOptions startupOptions)
        {
            ModManagerRoot = startupOptions.ModManagerPath;
            GameRoot = startupOptions.GamePath;
            Mods = startupOptions.ModInstallationPath;

            EnsurePathExists(Mods);
            EnsurePathExists(ModManager.Data);
            EnsurePathExists(ModManager.Temp);
            EnsurePathExists(ModManager.User);
        }

        private static void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GameRoot { get; private set; } = null!;

        public static string ModManagerRoot { get; private set; } = null!;

        public static string Mods { get; set; } = null!;

        public static class ModManager
        {
            public static string Data { get; } = Path.Combine(ModManagerRoot, "data");

            public static string User { get; } = Path.Combine(ModManagerRoot, "user");

            public static string Temp { get; } = Path.Combine(ModManagerRoot, "temp");
        }
    }
}