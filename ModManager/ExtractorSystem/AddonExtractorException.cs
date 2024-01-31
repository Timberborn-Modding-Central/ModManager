using System;

namespace ModManager.ExtractorSystem
{
    public class AddonExtractorException : Exception
    {
        public AddonExtractorException(string message) : base(message)
        {
        }
    }
}
