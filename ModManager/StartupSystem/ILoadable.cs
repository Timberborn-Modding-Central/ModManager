namespace ModManager.StartupSystem
{
    public interface ILoadable
    {
        public void Load(ModManagerStartupOptions startupOptions);
    }
}