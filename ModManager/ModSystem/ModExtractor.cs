using Modio.Models;
using ModManager.ExtractorSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ModManager.ModSystem
{
    public class ModExtractor : IAddonExtractor
    {
        private List<string> _foldersToIgnore = new() { "configs" };

        public bool Extract(string addonZipLocation, Mod modInfo, out string extractLocation, bool overWrite = true)
        {
            extractLocation = "";
            if (!modInfo.Tags.Any(x => x.Name == "Mod"))
            {
                return false;
            }
            var modFolderName = $"{modInfo.NameId}_{modInfo.Id}_{modInfo.Modfile.Version}";
            ClearOldModFiles(modInfo, modFolderName);
            extractLocation = Path.Combine(Paths.Mods, modFolderName);
            ZipFile.ExtractToDirectory(addonZipLocation, extractLocation, overWrite);
            System.IO.File.Delete(addonZipLocation);

            return true;
        }

        private void ClearOldModFiles(Mod modInfo, string modFolderName)
        {
            if (TryGetExistingModFolder(modInfo, out var dirs))
            {
                var dirInfo = new DirectoryInfo(dirs);
                if (dirInfo.Name.Equals(modFolderName))
                {
                    return;
                }
                dirInfo.MoveTo(Path.Combine(Paths.Mods, modFolderName));
                DeleteModFiles(modFolderName);
            }
        }
        private bool TryGetExistingModFolder(Mod modInfo, out string dirs)
        {
            dirs = null;
            try
            {
                dirs = Directory.GetDirectories(Paths.Mods, $"{modInfo.NameId}_{modInfo.Id}*").SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new AddonExtractorException($"Found multiple folders for \"{modInfo.Name}\"");
            }

            if (dirs != null)
            {
                return true;
            }

            return false;
        }
        
        private void DeleteModFiles(string modFolderName)
        {
            var modDirInfo = new DirectoryInfo(Path.Combine(Paths.Mods, modFolderName));
            var modSubFolders = modDirInfo
                .GetDirectories("*", SearchOption.AllDirectories)
                .Where(folder => !_foldersToIgnore.Contains(folder.FullName.Split(Path.DirectorySeparatorChar).Last()));
            foreach (var subDirectory in modSubFolders.Reverse())
            {
                DeleteFilesFromFolder(subDirectory);
                TryDeleteFolder(subDirectory);
            }

            DeleteFilesFromFolder(modDirInfo);
            TryDeleteFolder(modDirInfo);
        }

        private void DeleteFilesFromFolder(DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles().Where(file => !file.Name.EndsWith(Names.Extensions.Remove)))
            {
                try
                {
                    file.Delete();
                }
                catch (UnauthorizedAccessException ex)
                {
                    file.MoveTo($"{file.FullName}{Names.Extensions.Remove}");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void TryDeleteFolder(DirectoryInfo dir)
        {
            if (dir.EnumerateDirectories().Any() == false && dir.EnumerateFiles().Any() == false)
            {
                dir.Delete();
            }
        }
    }
}
