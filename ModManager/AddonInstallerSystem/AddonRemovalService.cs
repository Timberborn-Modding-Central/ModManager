using ModManager.StartupSystem;
using System;
using System.Collections.Generic;
using System.Text;

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
