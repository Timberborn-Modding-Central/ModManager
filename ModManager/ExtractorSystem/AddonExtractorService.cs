using Modio.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Linq;
using System.Diagnostics;
using ModManager.MapSystem;
using ModManager.ModIoSystem;
using ModManager.AddonInstallerSystem;

namespace ModManager.ExtractorSystem
{
    public class AddonExtractorService : Singleton<AddonExtractorService>
    {
        private readonly AddonExtractorRegistry _addonExtractorRegistry;

        public AddonExtractorService()
        {
            _addonExtractorRegistry = AddonExtractorRegistry.Instance;
        }

        public string Extract(Mod addonInfo, string addonZipLocation, bool overwrite = true)
        {
            foreach (IAddonExtractor extractor in _addonExtractorRegistry.GetAddonExtractor())
            {
                if (extractor.Extract(addonZipLocation, addonInfo, out string extractLocation))
                {
                    return extractLocation;
                }
            }

            throw new AddonExtractorException($"{addonInfo.Name} could not be installed by any extractor.");
        }
    }
}