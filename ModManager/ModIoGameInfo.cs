using ModManager.StartupSystem;

namespace ModManager
{
    public class ModIoGameInfo : Singleton<ModIoGameInfo>, ILoadable
    {
        public static string Url { get; private set; } = null!;

        public static uint GameId { get; private set; }

        public void Load(ModManagerStartupOptions startupOptions)
        {
            Url = startupOptions.ModIoGameUrl;
            GameId = startupOptions.GameId;
        }
    }
}