using System;
using System.Collections.Generic;
using System.Text;

namespace ModManager.ExtractorSystem
{
    public class ExtractorException : Exception
    {
        public ExtractorException(string message) : base(message)
        {
        }
    }
}
