using System.IO;
using Modio.Models;
using File = Modio.Models.File;

namespace ModManager.ModSystem
{
    public class InstalledMod
    {
        public static InstalledMod Create(Mod mod)
        {
            string installationPath = Path.Combine(Paths.ModInstallationFolder, mod.NameId!);
            return new InstalledMod(installationPath, false, mod.Modfile!, mod);
        }

        public InstalledMod(string installationPath, bool enabled, File currentModFile, Mod modObject)
        {
            InstallationPath = installationPath;
            Enabled = enabled;
            CurrentModFile = currentModFile;
            ModObject = modObject;
        }

        public string InstallationPath { get; set; }
        
        public bool Enabled { get; set; }
        
        public File CurrentModFile { get; set; }
        
        public Mod ModObject { get; set; }

        public void Enable()
        {

        }

        public void Disable()
        {
            
        }
    }
}