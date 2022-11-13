using System;

namespace ModManager.EnableSystem
{
    public class EnableServiceException : Exception
    {
        public EnableServiceException(string message) : base(message)
        {
        }
    }
}