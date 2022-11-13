using System.Collections.Generic;
using System.Linq;

namespace ModManager.AddonInstallerSystem
{
    public class AddonInstallerRegistry : Singleton<AddonInstallerRegistry>
    {
        private readonly List<KeyValuePair<string, IAddonInstaller>> _addonInstallers;

        public AddonInstallerRegistry()
        {
            _addonInstallers = new List<KeyValuePair<string, IAddonInstaller>>();
        }

        public void Add(string installerId, IAddonInstaller addonInstaller)
        {
            if (_addonInstallers.Exists(pair => pair.Key.Equals(installerId)))
            {
                throw new AddonInstallerException($"Addon installer with id: `{installerId}` is already added to the list");
            }

            _addonInstallers.Insert(0, new KeyValuePair<string, IAddonInstaller>(installerId, addonInstaller));
        }

        public void Remove(string installerId)
        {
            _addonInstallers.Remove(_addonInstallers.First(pair => pair.Key.Equals(installerId)));
        }

        public IEnumerable<IAddonInstaller> GetAddonInstallers()
        {
            return _addonInstallers.Select(pair => pair.Value);
        }
    }
}