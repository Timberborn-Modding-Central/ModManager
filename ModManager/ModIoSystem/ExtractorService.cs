using Modio.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Linq;

namespace ModManager.ModIoSystem
{
    public class ExtractorService : Singleton<ExtractorService>
    {
        private List<string> _foldersToIgnore = new() { "configs" };

        private const string _bepInExPackName = "BepInExPack";
        private const string _timberApiName = "TimberAPI";

        public string Extract(string mapZipLocation, Mod modInfo, bool overWrite = true)
        {
            if (modInfo.Tags.Any(x => x.Name.Equals("Map")))
            {
                return ExtractMap(mapZipLocation, modInfo, overWrite);
            }

            return ExtractMod(mapZipLocation, modInfo, overWrite);
        }

        public string ExtractMap(string mapZipLocation, Mod modInfo, bool overWrite = true)
        {
            var mapsInstallLocation = Paths.Maps;
            ZipFile.ExtractToDirectory(mapZipLocation, mapsInstallLocation, overWrite);
            System.IO.File.Delete(mapZipLocation);

            return mapsInstallLocation;
        }

        public string ExtractMod(string modZipLocation, Mod modInfo, bool overWrite = true)
        {
            string modFolderName = $"{modInfo.NameId}_{modInfo.Id}_{modInfo.Modfile.Version}";
            ClearOldModFiles(modInfo, modFolderName);

            string fullModPath = "";
            if (modInfo.Name.Equals(_bepInExPackName))
            {
                fullModPath = Path.Combine(Paths.GameRoot, "BepInEx");
            }
            else if(modInfo.Name.Equals(_timberApiName))
            {
                fullModPath = Path.Combine(Paths.Mods, modFolderName);
                ZipFile.ExtractToDirectory(modZipLocation, Paths.Mods, overWrite);
                Directory.Move(Path.Combine(Paths.Mods, _timberApiName), fullModPath);
            }
            else
            {
                fullModPath = Path.Combine(Paths.Mods, modFolderName);
                ZipFile.ExtractToDirectory(modZipLocation, fullModPath, overWrite);
            }

            System.IO.File.Delete(modZipLocation);
            return fullModPath;
        }

        private bool TryGetExistingModFolder(Mod modInfo, out string dirs)
        {
            dirs = null;
            try
            {
                dirs = Directory.GetDirectories(Paths.Mods, $"{modInfo.NameId}_{modInfo.Id}*").SingleOrDefault();
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
                dirInfo.MoveTo(Path.Combine(Paths.Mods, modFolderName));
                DeleteModFiles(modFolderName);
            }
        }

        private void DeleteModFiles(string modFolderName)
        {
            var modDirInfo = new DirectoryInfo(Path.Combine(Paths.Mods, modFolderName));
            var modSubFolders = modDirInfo.GetDirectories("*", SearchOption.AllDirectories)
                                          .Where(folder => !_foldersToIgnore.Contains(folder.FullName.Split(Path.DirectorySeparatorChar).Last()));
            foreach (DirectoryInfo subDirectory in modSubFolders.Reverse())
            {
                DeleteFilesFromFolder(subDirectory);
                TryDeleteFolder(subDirectory);
            }

            DeleteFilesFromFolder(modDirInfo);
            TryDeleteFolder(modDirInfo);
        }

        private void DeleteFilesFromFolder(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles().Where(file => !file.Name.EndsWith(Names.Extensions.Remove)))
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