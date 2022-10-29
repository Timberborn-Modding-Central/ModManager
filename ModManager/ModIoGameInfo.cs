using ModManagerWrapper.StartupSystem;

namespace ModManagerWrapper
{
    public class ModIoGameInfo : ILoadable
    {
        public string Url { get; set; }
        
        public uint GameId { get; private set; }


        public void Load(ModManagerStartupOptions startupOptions)
        {

        }
    }
}