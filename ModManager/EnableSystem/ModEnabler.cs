using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModManager.ModSystem;
using ModManager.SingletonInstanceSystem;

namespace ModManager.EnableSystem
{
    public class ModEnabler : Singleton<ModEnabler>
    {
        public static readonly string DisabledExtension = ".disabled";

        private static readonly IEnumerable<string> IgnoreExtensions = new[]
        {
            DisabledExtension,
            ".delete"
        };

        public void Enable(Manifest manifest)
        {
            string[] disabledFilePaths = Directory.GetFiles(manifest.RootPath, "*.disabled", SearchOption.AllDirectories);

            foreach (string filePath in disabledFilePaths)
            {
                string enabledFilePath = filePath.Remove(filePath.Length - DisabledExtension.Length, DisabledExtension.Length);

                File.Move(filePath, enabledFilePath);
            }

            manifest.Enabled = true;
        }

        public void Disable(Manifest manifest)
        {
            string[] filePaths = Directory.GetFiles(manifest.RootPath, "*", SearchOption.AllDirectories);

            string[] filePathsWithoutExcludedExtensions = filePaths.Where(filePath => ! IgnoreExtensions.Contains(Path.GetExtension(filePath))).ToArray();

            foreach (string filePath in filePathsWithoutExcludedExtensions)
            {
                File.Move(filePath, filePath + DisabledExtension);
            }

            manifest.Enabled = false;
        }
    }
}