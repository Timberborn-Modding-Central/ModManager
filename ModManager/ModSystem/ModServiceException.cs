using System;

namespace ModManager.ModSystem
{
    public class ModServiceException : Exception
    {
        public ModServiceException(string message) : base(message)
        {
        }
    }
}