using System;
using System.Collections.Generic;
using Modio.Models;
using ModManager.ModSystem;
using ModManager.SingletonInstanceSystem;

namespace ModManager.InstallerSystem
{
    public class InstallerService : Singleton<InstallerService>
    {
        private readonly List<IModInstaller> _installers = new();

        public string Install(Mod mod, File file)
        {
            foreach (IModInstaller installer in _installers)
            {
                if (!installer.Install(mod, file, out string? installationPath))
                {
                    continue;
                }

                if (installationPath == null)
                {
                    throw new Exception($"Installer: {installer.GetType()} did not set a installation path");
                }

                return installationPath;
            }

            throw new Exception($"{mod.Name} could not be installed by any installer");
        }

        public void Uninstall(Mod mod, Manifest manifest)
        {
            foreach (IModInstaller installer in _installers)
            {
                if (!installer.Uninstall(mod, manifest))
                {
                    continue;
                }

                return;
            }

            throw new Exception($"{mod.Name} could not be uninstalled by any installer");
        }

        public string ChangeVersion(Mod mod, File file)
        {
            foreach (IModInstaller installer in _installers)
            {
                if (!installer.ChangeVersion(mod, file, out string? installationPath))
                {
                    continue;
                }

                if (installationPath == null)
                {
                    throw new Exception($"Installer: {installer.GetType()} did not set a installation path");
                }

                return installationPath;
            }

            throw new Exception($"The version of {mod.Name} could not be changed by any installer");
        }

        public void AddInstaller(IModInstaller installer)
        {
            _installers.Insert(0, installer);
        }
    }
}