using Modio.Models;

namespace ModManager.ExtractorSystem
{
    public class AddonExtractorService : Singleton<AddonExtractorService>
    {
        private readonly AddonExtractorRegistry _addonExtractorRegistry = AddonExtractorRegistry.Instance;

        public string Extract(Mod addonInfo, string addonZipLocation, bool overwrite = true)
        {
            foreach (var extractor in _addonExtractorRegistry.GetAddonExtractor())
            {
                if (extractor.Extract(addonZipLocation, addonInfo, out var extractLocation))
                {
                    return extractLocation;
                }
            }

            throw new AddonExtractorException($"{addonInfo.Name} could not be installed by any extractor.");
        }
    }
}