using System;
using System.IO;
using System.Linq;
using Modio.Models;
using ModManager.AddonInstallerSystem;
using ModManager.AddonSystem;
using ModManager.ExtractorSystem;
using ModManager.MapSystem;
using ModManager.PersistenceSystem;

namespace ModManager.ModSystem
{
    public class ModInstaller : IAddonInstaller
    {
        private readonly PersistenceService _persistenceService;

        private readonly InstalledAddonRepository _installedAddonRepository;

        private readonly AddonExtractorService _extractor;

        public ModInstaller()
        {
            _persistenceService = PersistenceService.Instance;
            _installedAddonRepository = InstalledAddonRepository.Instance;
            _extractor = AddonExtractorService.Instance;
        }

        public bool Install(Mod mod, string zipLocation)
        {
            if (!mod.Tags.Any(x => x.Name == "Mod"))
            {
                return false;
            }
            string installLocation = _extractor.Extract(mod, zipLocation);
            var manifest = new Manifest(mod, mod.Modfile, installLocation);
            string modManifestPath = Path.Combine(installLocation, Manifest.FileName);
            _persistenceService.SaveObject(manifest, modManifestPath);
            _installedAddonRepository.Add(manifest);

            return true;
        }

        public bool Uninstall(Manifest manifest)
        {
            if (manifest is not Manifest && manifest is MapManifest)
            {
                return false;
            }
            _installedAddonRepository.Remove(manifest.ModId);

            var modDirInfo = new DirectoryInfo(Path.Combine(manifest.RootPath));
            var modSubFolders = modDirInfo.GetDirectories("*", SearchOption.AllDirectories);
            foreach (DirectoryInfo subDirectory in modSubFolders.Reverse())
            {
                DeleteFilesFromFolder(subDirectory);
                TryDeleteFolder(subDirectory);
            }

            DeleteFilesFromFolder(modDirInfo);
            TryDeleteFolder(modDirInfo);
            return true;
        }

        public bool ChangeVersion(Mod mod, Modio.Models.File file, string zipLocation)
        {
            if (!mod.Tags.Any(x => x.Name == "Mod"))
            {
                return false;
            }
            mod.Modfile= file;
            string installLocation = _extractor.Extract(mod, zipLocation);
            var manifest = new Manifest(mod, mod.Modfile, installLocation);
            string modManifestPath = Path.Combine(installLocation, Manifest.FileName);
            _persistenceService.SaveObject(manifest, modManifestPath);

            _installedAddonRepository.Remove(mod.Id);
            _installedAddonRepository.Add(manifest);

            return true;
        }


        private void DeleteFilesFromFolder(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch(UnauthorizedAccessException ex)
                {
                    file.MoveTo($"{file.FullName}{Names.Extensions.Remove}");
                }
                catch(Exception ex)
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
                else
                {
                    dir.MoveTo($"{dir.FullName}{Names.Extensions.Remove}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}