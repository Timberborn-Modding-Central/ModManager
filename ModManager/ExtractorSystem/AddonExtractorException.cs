using System;
using System.Collections.Generic;
using System.Text;

namespace ModManager.ExtractorSystem
{
    public class AddonExtractorException : Exception
    {
        public AddonExtractorException(string message) : base(message)
        {
        }
    }
}
