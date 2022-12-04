using System;

namespace ModManager.ManifestValidatorSystem
{
    public class ManifestValidatorException : Exception
    {
        public ManifestValidatorException(string message) : base(message)
        {
        }
    }
}
