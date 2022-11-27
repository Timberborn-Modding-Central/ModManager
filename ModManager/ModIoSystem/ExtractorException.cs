using System;
using System.Collections.Generic;
using System.Text;

namespace ModManager.ModIoSystem
{
    public class ExtractorException : Exception
    {
        public ExtractorException(string message) : base(message)
        {
        }
    }
}
