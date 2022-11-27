using Modio.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Linq;
using System.Diagnostics;
using ModManager.MapSystem;

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
            var zipFile = ZipFile.OpenRead(mapZipLocation);
            var timberFile = zipFile.Entries
                                    .Where(x => x.Name.Contains(".timber"))
                                    .SingleOrDefault() ?? throw new MapException("Map zip does not contain an entry for a .timber file");

            timberFile.ExtractToFile(Path.Combine(Paths.Maps, timberFile.Name));
            zipFile.Dispose();

            var mapsInstallLocation = Paths.Maps;

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
            catch (InvalidOperationException ex)
            {
                throw new ExtractorException($"Found multiple folders for \"{modInfo.Name}\"");
            }
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}