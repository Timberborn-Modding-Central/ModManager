﻿using ModManager.AddonEnableSystem;
using ModManager.AddonInstallerSystem;
using ModManager.ManifestLocationFinderSystem;
using ModManager.StartupSystem;

namespace ModManager.MapSystem
{
    public class MapRegisterer : Singleton<MapRegisterer>, ILoadable
    {
        public const string RegistryId = "Map";

        public void Load(ModManagerStartupOptions startupOptions)
        {
            AddonInstallerRegistry.Instance.Add(RegistryId, new MapInstaller());
            AddonEnablerRegistry.Instance.Add(RegistryId, new MapEnabler());
            ManifestLocationFinderRegistry.Instance.Add(RegistryId, new MapManifestFinder());
        }
    }
}