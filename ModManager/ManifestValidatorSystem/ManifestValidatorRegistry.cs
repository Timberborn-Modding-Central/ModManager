using System.Collections.Generic;
using System.Linq;

namespace ModManager.ManifestValidatorSystem
{
    public class ManifestValidatorRegistry : Singleton<ManifestValidatorRegistry>
    {
        private readonly List<KeyValuePair<string, IManifestValidator>> _manifestValidators;

        public ManifestValidatorRegistry()
        {
            _manifestValidators = new List<KeyValuePair<string, IManifestValidator>>();
        }

        public void Add(string validatorId, IManifestValidator manifestValidator)
        {
            if (_manifestValidators.Exists(pair => pair.Key.Equals(validatorId)))
            {
                throw new ManifestValidatorException($"Manifest validator with id: `{validatorId}` is already added to the list");
            }

            _manifestValidators.Insert(0, new KeyValuePair<string, IManifestValidator>(validatorId, manifestValidator));
        }

        public void Remove(string installerId)
        {
            _manifestValidators.Remove(_manifestValidators.First(pair => pair.Key.Equals(installerId)));
        }

        public IEnumerable<IManifestValidator> GetManifestValidator()
        {
            return _manifestValidators.Select(pair => pair.Value);
        }
    }
}
