using System;

namespace ModManager.AddonSystem
{
    public class AddonException : Exception
    {
        public AddonException(string message) : base(message)
        {
        }
    }
}