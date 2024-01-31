using System.IO;
using ModManager.AddonSystem;
using ModManager.PersistenceSystem;

namespace ModManager.StartupSystem
{
    public class RemoveFilesOnStartup : Singleton<RemoveFilesOnStartup>, ILoadable
    {
        private readonly InstalledAddonRepository _addonRepository = InstalledAddonRepository.Instance;

        private readonly PathRemovalService _removalService = PathRemovalService.Instance;

        public void Load(ModManagerStartupOptions startupOptions)
        {
            DeleteRemoveTaggedFiles(Paths.GameRoot);
            DeleteRemoveTaggedDirectories(Paths.GameRoot);

            foreach (var manifest in _addonRepository.All())
            {
                DeleteRemoveTaggedFiles(manifest.RootPath);
                _removalService.TryDeleteEmptyDictionary(manifest.RootPath);
            }
        }

        private void DeleteRemoveTaggedFiles(string path)
        {
            foreach (var removableFilePaths in Directory.GetFiles(path, $"*{Names.Extensions.Remove}", SearchOption.AllDirectories))
            {
                _removalService.TryDeleteFile(removableFilePaths);
            }
        }

        private void DeleteRemoveTaggedDirectories(string path)
        {
            foreach (var removableFilePaths in Directory.GetDirectories(path, $"*{Names.Extensions.Remove}", SearchOption.AllDirectories))
            {
                _removalService.TryDeleteEmptyDictionary(removableFilePaths);
            }
        }
    }
}