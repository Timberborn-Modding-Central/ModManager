using ModManager.AddonInstallerSystem;
using System.Collections.Generic;
using System.Linq;

namespace ModManager.ExtractorSystem
{
    public class AddonExtractorRegistry : Singleton<AddonExtractorRegistry>
    {
        private readonly List<KeyValuePair<string, IAddonExtractor>> _addonExtractors;

        public AddonExtractorRegistry()
        {
            _addonExtractors = new List<KeyValuePair<string, IAddonExtractor>>();
        }

        public void Add(string extractorId, IAddonExtractor addonExtractor)
        {
            if (_addonExtractors.Exists(pair => pair.Key.Equals(extractorId)))
            {
                throw new AddonInstallerException($"Addon extractor with id: `{extractorId}` is already added to the list");
            }

            _addonExtractors.Insert(0, new KeyValuePair<string, IAddonExtractor>(extractorId, addonExtractor));
        }

        public void Remove(string installerId)
        {
            _addonExtractors.Remove(_addonExtractors.First(pair => pair.Key.Equals(installerId)));
        }

        public IEnumerable<IAddonExtractor> GetAddonExtractor()
        {
            return _addonExtractors.Select(pair => pair.Value);
        }
    }
}
