using Modio.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Timberborn.MapSystem;
using System.Linq;

namespace ModManager.ModIoSystem
{
    public class ExtractorService
    {
        private List<string> _foldersToIgnore = new() { "configs" };

        private const string _bepInExPackName = "BepInExPack";

        public void Extract(string mapZipLocation, Mod modInfo, bool overWrite = true)
        {
            if (modInfo.Tags.Any(x => x.Name.Equals("Map")))
            {
                ExtractMap(mapZipLocation, modInfo, overWrite);
                return;
            }

            ExtractMod(mapZipLocation, modInfo, overWrite);
        }

        private void ExtractMap(string mapZipLocation, Mod modInfo, bool overWrite = true)
        {
            ZipFile.ExtractToDirectory(mapZipLocation, MapRepository.CustomMapsDirectory, overWrite);
            System.IO.File.Delete(mapZipLocation);
        }

        private void ExtractMod(string modZipLocation, Mod modInfo, bool overWrite = true)
        {
            string modFolderName = $"{modInfo.NameId}_{modInfo.Id}_{modInfo.Modfile.Version}";
            ClearOldModFiles(modInfo, modFolderName);

            if (modInfo.Name.Equals(_bepInExPackName))
            {
                // TODO: Better way to get folders
                ZipFile.ExtractToDirectory(modZipLocation, Path.Combine(Paths.Timberborn, "BepInEx", "plugins", modFolderName), overWrite);
            }
            else
            {
                ZipFile.ExtractToDirectory(modZipLocation, Path.Combine(Paths.Timberborn, "mods", modFolderName), overWrite);
            }

            System.IO.File.Delete(modZipLocation);
        }

        private bool TryGetExistingModFolder(Mod modInfo, out string dirs)
        {
            dirs = null;
            try
            {
                dirs = Directory.GetDirectories(Path.Combine(Paths.Timberborn, "mods"), $"{modInfo.NameId}_{modInfo.Id}*").SingleOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Found multiple folders for for \"{modInfo.Name}\"");
                Console.WriteLine($"{ex.Message}");
            }
            if (dirs != null)
            {
                return true;
            }

            return false;
        }

        private void ClearOldModFiles(Mod modInfo, string modFolderName)
        {
            if (TryGetExistingModFolder(modInfo, out string dirs))
            {
                var dirInfo = new DirectoryInfo(dirs);
                if (dirInfo.Name.Equals(modFolderName))
                {
                    return;
                }
                dirInfo.MoveTo(Path.Combine(Paths.Data, modFolderName));
                DeleteModFiles(modFolderName);
            }
        }

        private void DeleteModFiles(string modFolderName)
        {
            var modDirInfo = new DirectoryInfo(Path.Combine(Paths.Data, modFolderName));
            var modSubFolders = modDirInfo.GetDirectories("*", SearchOption.AllDirectories)
                                          .Where(file => !_foldersToIgnore.Contains(file.FullName.Split(Path.DirectorySeparatorChar).Last()));
            foreach (DirectoryInfo subDirectory in modSubFolders.Reverse())
            {
                DeleteFilesFromFolder(subDirectory);
                TryDeleteFolder(subDirectory);
            }

            DeleteFilesFromFolder(modDirInfo);
            TryDeleteFolder(modDirInfo);
            //Console.WriteLine($"Deleted everything expect for {_foldersToIgnore.Aggregate((a, b) => $"{a}, {b}")}");
        }

        private void DeleteFilesFromFolder(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }

        private void TryDeleteFolder(DirectoryInfo dir)
        {
            try
            {
                if (dir.EnumerateDirectories().Any() == false && dir.EnumerateFiles().Any() == false)
                {
                    dir.Delete();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"\t\tIO exc: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\t\texc: {ex.Message}");
            }
        }
    }
}