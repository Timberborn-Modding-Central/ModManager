using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;
using ModManager.MapSystem;
using ModManager.ModIoSystem;
using ModManager.PersistenceSystem;

namespace ModManager.ModSystem
{
    public class ModInstaller : IAddonInstaller
    {
        private readonly PersistenceService _persistenceService;

        private readonly InstalledAddonRepository _installedAddonRepository;

        private readonly ExtractorService _extractor;

        public ModInstaller()
        {
            _persistenceService = PersistenceService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
            _extractor = ExtractorService.Instance;
        }

        public bool Install(Mod mod, string zipLocation)
        {
            if (!mod.Tags.Any(x => x.Name == "Mod"))
            {
                return false;
            }
            string installLocation = _extractor.ExtractMod(zipLocation, mod);
            var manifest = new Manifest(mod, mod.Modfile, installLocation);
            string modManifestPath = Path.Combine(installLocation, Manifest.FileName);
            _persistenceService.SaveObject(manifest, modManifestPath);
            _installedAddonRepository.Add(manifest);

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            Console.WriteLine($"inside modinstaller");
            if (manifest is not Manifest && manifest is MapManifest)
            {
                Console.WriteLine($"\"{manifest.ModName} is not a mod.");
                return false;
            }
            _installedAddonRepository.Remove(manifest.ModId);

            MarkDllAndManifestFilesForDeletion(manifest);



            try
            {
                //Directory.Delete(manifest.RootPath, true);
            }
            catch(UnauthorizedAccessException ex)
            {
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public bool ChangeVersion(Mod mod, Modio.Models.File file, string zipLocation)
        {
            throw new NotImplementedException();
        }

        private void MarkDllAndManifestFilesForDeletion(Manifest manifest)
        {
            var dirInfo = new DirectoryInfo(manifest.RootPath);
            var dllFiles = dirInfo.GetFiles("*.dll", SearchOption.AllDirectories);
            foreach (var dllfile in dllFiles)
            {
                dllfile.MoveTo($"dllfile.Name.{Names.Extensions.Remove}");
            }
            var manifestFullPath = Path.Combine(manifest.RootPath, Manifest.FileName);
            var fileInfo = new FileInfo(manifestFullPath);
            if(fileInfo.Exists)
            {
                fileInfo.MoveTo($"{Manifest.FileName}.{Names.Extensions.Remove}");
            }
        }
    }
}