using System;
using System.IO;
using System.Net.Mime;
using System.Runtime.InteropServices;
using ModManager.StartupSystem;

namespace ModManager
{
    public class Paths : Singleton<Paths>, ILoadable
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

        public static string GameRoot { get; set; } = null!;

        public static string ModManagerRoot { get; set; } = null!;

        public static string Mods { get; set; } = null!;

        public static readonly string Maps = Path.Combine(RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents", "Timberborn") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Timberborn"), "Maps");

        public static class ModManager
        {
            public static string Data { get; set; } = Path.Combine(ModManagerRoot, "data");

            public static string User { get; set; } = Path.Combine(ModManagerRoot, "user");

            public static string Temp { get; set; } = Path.Combine(ModManagerRoot, "temp");

            public static string Assets { get; set; } = Path.Combine(ModManagerRoot, "assets");

            public static string Lang { get; set; } = Path.Combine(ModManagerRoot, "lang");
        }
    }
}