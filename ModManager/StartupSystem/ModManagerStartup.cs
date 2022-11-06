using System;
using System.Collections.Generic;
using ModManager.ModIoSystem;
using ModManager.ModSystem;

namespace ModManager.StartupSystem
{
    public class ModManagerStartup
    {
        public static bool IsLoaded;

        private static ModManagerStartup? _instance;

        private static ModManagerStartup Instance => _instance ??= new ModManagerStartup();

        private readonly IEnumerable<ILoadable> _loadableClasses = new List<ILoadable>
        {
            Paths.Instance,
            ModIoGameInfo.Instance,
            InstalledModRepository.Instance
        };

        public static void Run(string apiKey, Action<ModManagerStartupOptions> options)
        {
            var modManagerOptions = new ModManagerStartupOptions();

            options(modManagerOptions);

            ModIo.InitializeClient(apiKey);

            IsLoaded = true;

            Instance.LoadClasses(modManagerOptions);
        }

        private void LoadClasses(ModManagerStartupOptions startupOptions)
        {
            foreach (ILoadable loadableClass in _loadableClasses)
            {
                loadableClass.Load(startupOptions);
            }
        }
    }
}