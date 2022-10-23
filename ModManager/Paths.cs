using System.IO;
using System.Reflection;

namespace ModManager
{
    public static class Paths
    {
        public static void LoadPaths()
        {
            ModManagerDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            GameDirectory = BepInEx.Paths.GameRootPath;
            ModInstallationFolder = Path.Combine(GameDirectory, "mods");

            EnsurePathExists(ModInstallationFolder);
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

        public static string GameDirectory { get; private set; } = null!;

        public static string ModManagerDirectory { get; private set; } = null!;

        public static string ModInstallationFolder { get; set; } = null!;

        public static class ModManager
        {
            public static string Data { get; } = Path.Combine(ModManagerDirectory, "data");

            public static string User { get; } = Path.Combine(ModManagerDirectory, "user");

            public static string Temp { get; } = Path.Combine(ModManagerDirectory, "temp");
        }
    }
}