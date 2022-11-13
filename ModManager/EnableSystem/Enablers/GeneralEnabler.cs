using System.IO;
using System.Linq;
using ModManager.ModSystem;

namespace ModManager.EnableSystem.Enablers
{
    public class GeneralEnabler : IModEnabler
    {
        public bool Enable(Manifest manifest)
        {
            string[] disabledFilePaths = Directory.GetFiles(manifest.RootPath, "*" + Names.Extensions.Disabled, SearchOption.AllDirectories);

            foreach (string filePath in disabledFilePaths)
            {
                string enabledFilePath = filePath.Remove(filePath.Length - Names.Extensions.Disabled.Length, Names.Extensions.Disabled.Length);

                File.Move(filePath, enabledFilePath);
            }

            manifest.Enabled = true;

            return true;
        }

        public bool Disable(Manifest manifest)
        {
            string[] filePaths = Directory.GetFiles(manifest.RootPath, "*", SearchOption.AllDirectories);

            string[] filePathsWithoutExcludedExtensions = filePaths.Where(filePath => ! ModEnableService.IgnoreExtensions.Contains(Path.GetExtension(filePath))).ToArray();

            foreach (string filePath in filePathsWithoutExcludedExtensions)
            {
                File.Move(filePath, filePath + Names.Extensions.Disabled);
            }

            manifest.Enabled = false;

            return true;
        }
    }
}