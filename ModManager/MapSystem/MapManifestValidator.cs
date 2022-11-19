using ModManager.PersistenceSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModManager.MapSystem
{
    // TODO: Idea is that this validates manifests on startup and removes invalid manifests
    //       Is this needed?
    //       Probably good to have atleast in someone manually deleted maps
    public class MapManifestValidator
    {
        private readonly PersistenceService _persistenceService;

        public MapManifestValidator()
        {
            _persistenceService = PersistenceService.Instance;
        }
        public void ValidateMapManifest(List<MapManifest> mapManifests)
        {

        }
    }
}
