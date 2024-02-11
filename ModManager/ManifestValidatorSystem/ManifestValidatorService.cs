namespace ModManager.ManifestValidatorSystem
{
    public class ManifestValidatorService : Singleton<ManifestValidatorService>
    {
        private readonly ManifestValidatorRegistry _manifestValidatorRegistry = ManifestValidatorRegistry.Instance;

        public void ValidateManifests()
        {
            foreach (var validator in _manifestValidatorRegistry.GetManifestValidator())
            {
                validator.ValidateManifests();
            }
        }
    }
}
