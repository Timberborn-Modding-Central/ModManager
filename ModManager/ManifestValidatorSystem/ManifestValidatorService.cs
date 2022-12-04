namespace ModManager.ManifestValidatorSystem
{
    public class ManifestValidatorService : Singleton<ManifestValidatorService>
    {
        private readonly ManifestValidatorRegistry _manifestValidatorRegistry;

        public ManifestValidatorService()
        {
            _manifestValidatorRegistry = ManifestValidatorRegistry.Instance;
        }

        public void ValidateManifests()
        {
            foreach (IManifestValidator validator in _manifestValidatorRegistry.GetManifestValidator())
            {
                validator.ValidateManifests();
            }
        }
    }
}
