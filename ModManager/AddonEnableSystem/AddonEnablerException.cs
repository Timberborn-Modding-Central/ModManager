using System;

namespace ModManager.AddonEnableSystem
{
    public class AddonEnablerException : Exception
    {
        public AddonEnablerException(string message) : base(message)
        {
        }
    }
}