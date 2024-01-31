using ModManager.StartupSystem;
using System;

namespace ModManager.AddonInstallerSystem
{
    public class AddonRemovalService : Singleton<AddonRemovalService>, ILoadable
    {
        public AddonRemovalService()
        {
        }

        public void Load(ModManagerStartupOptions startupOptions)
        {
            throw new NotImplementedException();
        }
    }
}
