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
            DeleteZipFile(mapZipLocation);
        }

        private void ExtractMod(string modZipLocation, Mod modInfo, bool overWrite = true)
        {
            string modFolderName = $"{modInfo.NameId}_{modInfo.Id}_{modInfo.Modfile.Version}";

            string dirs = null;
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
                var dirInfo = new DirectoryInfo(dirs);
                if (dirInfo.Name.Equals(modFolderName))
                {
                    return;
                }
                dirInfo.MoveTo(Path.Combine(Paths.Data, modFolderName));
                DeleteOldModFiles(modFolderName);
            }

            if (modInfo.Name.Equals(_bepInExPackName))
            {
                // TODO: Better way to get folders
                ZipFile.ExtractToDirectory(modZipLocation, Path.Combine(Paths.Timberborn, "BepInEx", "plugins", modFolderName), overWrite);
                Console.WriteLine($"Extracted to {Path.Combine(Paths.Timberborn, "BepInEx", "plugins", modFolderName)}");
            }
            else
            {
                ZipFile.ExtractToDirectory(modZipLocation, Path.Combine(Paths.Timberborn, "mods", modFolderName), overWrite);
                Console.WriteLine($"Extracted to {Path.Combine(Paths.Timberborn, "mods", modFolderName)}");
            }

            DeleteZipFile(modZipLocation);
        }

        private void DeleteOldModFiles(string modFolderName)
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

        private void DeleteZipFile(string mapZipLocation)
        {
            System.IO.File.Delete(mapZipLocation);
        }
    }
}