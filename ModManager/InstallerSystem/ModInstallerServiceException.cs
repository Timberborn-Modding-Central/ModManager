using System;

namespace ModManager.InstallerSystem
{
    public class ModInstallerServiceException : Exception
    {
        public ModInstallerServiceException(string message) : base(message)
        {
        }
    }
}