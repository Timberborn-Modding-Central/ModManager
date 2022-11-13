using System;

namespace ModManager.ManifestLocationFinderSystem
{
    public class ManifestException : Exception
    {
        public ManifestException(string message) : base(message)
        {
        }
    }
}