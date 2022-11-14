using System;

namespace ModManager.AddonInstallerSystem
{
    public class AddonInstallerException : Exception
    {
        public AddonInstallerException(string message) : base(message)
        {
        }
    }
}