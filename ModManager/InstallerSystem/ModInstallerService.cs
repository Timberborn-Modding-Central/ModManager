using System;
using System.Collections.Generic;
using Modio.Models;
using ModManager.InstallerSystem.Installers;
using ModManager.ModSystem;
using ModManager.SingletonInstanceSystem;

namespace ModManager.InstallerSystem
{
    public class ModInstallerService : Singleton<ModInstallerService>
    {
        private readonly List<IModInstaller> _installers = new()
        {
            new GeneralInstaller()
        };

        public void Install(Mod mod, File file)
        {
            foreach (IModInstaller installer in _installers)
            {
                if (installer.Install(mod, file))
                {
                    return;
                }
            }

            throw new Exception($"{mod.Name} could not be installed by any installer");
        }

        public void Uninstall(Manifest manifest)
        {
            foreach (IModInstaller installer in _installers)
            {
                if (installer.Uninstall(manifest))
                {
                    return;
                }
            }

            throw new Exception($"{manifest.ModName} could not be uninstalled by any installer");
        }

        public void ChangeVersion(Mod mod, File file)
        {
            foreach (IModInstaller installer in _installers)
            {
                if (installer.ChangeVersion(mod, file))
                {
                    return;
                }
            }

            throw new Exception($"The version of {mod.Name} could not be changed by any installer");
        }

        public void AddInstaller(IModInstaller installer)
        {
            _installers.Insert(0, installer);
        }
    }
}