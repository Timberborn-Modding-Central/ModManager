using System.Collections.Generic;
using System.Linq;

namespace ModManager.AddonEnableSystem
{
    public class AddonEnablerRegistry : Singleton<AddonEnablerRegistry>
    {
        private readonly List<KeyValuePair<string, IAddonEnabler>> _addonEnablers = new();

        public void Add(string enablerId, IAddonEnabler enabler)
        {
            if (_addonEnablers.Exists(pair => pair.Key.Equals(enablerId)))
            {
                throw new AddonEnablerException($"Enabler with id: `{enablerId}` is already added to the list");
            }

            _addonEnablers.Insert(0, new KeyValuePair<string, IAddonEnabler>(enablerId, enabler));
        }

        public void Remove(string enablerId)
        {
            _addonEnablers.Remove(_addonEnablers.First(pair => pair.Key.Equals(enablerId)));
        }

        public IEnumerable<IAddonEnabler> GetAddonEnablers()
        {
            return _addonEnablers.Select(pair => pair.Value);
        }
    }
}