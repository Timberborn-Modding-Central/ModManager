using System;
using System.Collections.Generic;
using Modio;
using ModManagerWrapper.ModSystem;

namespace ModManagerWrapper.StartupSystem
{
    public class ModManagerStartup
    {
        private static readonly IEnumerable<ILoadable> LoadableClasses = new List<ILoadable>()
        {
            new Paths(),
            new InstalledModRepository()
        };

        public static void Run(Client modIoClient, Action<ModManagerStartupOptions> options)
        {
            var modManagerOptions = new ModManagerStartupOptions();
            options(modManagerOptions);

            LoadClasses(modManagerOptions);
        }

        private static void LoadClasses(ModManagerStartupOptions startupOptions)
        {
            foreach (ILoadable loadableClass in LoadableClasses)
            {
                loadableClass.Load(startupOptions);
            }
        }
    }
}