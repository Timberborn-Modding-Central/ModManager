﻿using ModManager.AddonSystem;
using ModManager.ManifestValidatorSystem;
using ModManager.PersistenceSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModManager.MapSystem
{
    public class MapManifestValidator : IManifestValidator
    {
        private readonly PersistenceService _persistenceService;

        public MapManifestValidator()
        {
            _persistenceService = PersistenceService.Instance;
        }

        public void ValidateManifests()
        {
            var mapManifestFinder = new MapManifestFinder();
            var mapManifests = mapManifestFinder.Find().Select(a => (MapManifest)a).ToList();
            int oldManifestCount = mapManifests.Count;

            foreach (MapManifest mapManifest in mapManifests.ToList())
            {
                var fileName = Path.Combine(Paths.Maps, $"{mapManifest.MapFileName}{Names.Extensions.TimberbornMap}");
                if (!File.Exists(fileName))
                {
                    mapManifests.Remove(mapManifest);
                }
            }
            int newManifestCount = mapManifests.Count;

            if(oldManifestCount != newManifestCount )
            {
                string mapManifestPath = Path.Combine(Paths.Maps, MapManifest.FileName);
                _persistenceService.SaveObject(mapManifests, mapManifestPath);
            }
        }
    }
}
