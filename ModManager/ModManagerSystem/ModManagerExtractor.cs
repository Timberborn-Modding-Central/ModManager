using Modio.Models;
using ModManager.ExtractorSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace ModManager.ModManagerSystem
{
    public class ModManagerExtractor : IAddonExtractor
    {
        private string _modManagerFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private const string _modManagerPackageName = "Mod Manager";
        private List<string> _foldersToIgnore = new() { "temp" };

        public bool Extract(string addonZipLocation, Mod modInfo, out string extractLocation, bool overWrite = true)
        {
            extractLocation = "";
            if (modInfo.Name != _modManagerPackageName)
            {
                return false;
            }
            ClearOldModFiles(_modManagerFolderPath);
            extractLocation = _modManagerFolderPath;
            ZipFile.ExtractToDirectory(addonZipLocation, Paths.Mods, overWrite);
            System.IO.File.Delete(addonZipLocation);

            return true;
        }
        
        private void ClearOldModFiles(string modFolderName)
        { 
            DeleteModFiles(modFolderName);
        }

        private void DeleteModFiles(string modFolderName)
        {
            var modDirInfo = new DirectoryInfo(Path.Combine(Paths.Mods, modFolderName));
            var modSubFolders = 
                modDirInfo.GetDirectories("*", SearchOption.AllDirectories)
                          .Where(folder => !_foldersToIgnore.Contains(folder.FullName
                                                            .Split(Path.DirectorySeparatorChar)
                                                            .Last()));
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
                    if(!file.Name.EndsWith(".dll"))
                    {
                        continue;
                    }
                    file.Delete();
                }
                catch (UnauthorizedAccessException ex)
                {
                    file.MoveTo($"{file.FullName}{Names.Extensions.Remove}");
                }
                catch (IOException ex)
                {
                    try
                    {
                        file.MoveTo($"{file.FullName}{Names.Extensions.Remove}");
                    }
                    catch(IOException ex2)
                    {
                        throw;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
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
