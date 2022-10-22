using System.IO;
using System.Reflection;

namespace ModManager
{
    public static class Paths
    {
        public static void LoadPaths()
        {
            ModManager = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            Data = Path.Combine(ModManager, "data");

            Timberborn = BepInEx.Paths.GameRootPath;

            EnsurePathExists(Data);
        }

        private static void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string ModManager { get; private set; } = null!;

        public static string Data { get; private set; } = null!;

        public static string Timberborn { get; private set; } = null!;
    }
}