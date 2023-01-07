using ModManager.LoggingSystem;

namespace ModManager.StartupSystem
{
    public class ModManagerStartupOptions
    {
        public string ModIoGameUrl { get; set; } = null!;

        public uint GameId { get; set; }

        public bool IsGameRunning { get; set; } = false;

        public string ModInstallationPath { get; set; } = null!;

        public string GamePath { get; set; } = null!;

        public string ModManagerPath { get; set; } = null!;

        public IModManagerLogger Logger { get; set; } = null!;
    }
}