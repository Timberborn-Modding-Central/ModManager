using System.IO;
using ModManager.AddonSystem;
using ModManager.PersistenceSystem;

namespace ModManager.StartupSystem
{
    public class RemoveFilesOnStartup : Singleton<RemoveFilesOnStartup>, ILoadable
    {
        private readonly InstalledAddonRepository _addonRepository;

        private readonly PathRemovalService _removalService;

        public RemoveFilesOnStartup()
        {
            _addonRepository = InstalledAddonRepository.Instance;
            _removalService = PathRemovalService.Instance;
        }

        public void Load(ModManagerStartupOptions startupOptions)
        {
            DeleteRemoveTaggedFiles(Paths.GameRoot);

            foreach (Manifest manifest in _addonRepository.All())
            {
                DeleteRemoveTaggedFiles(manifest.RootPath);
                _removalService.TryDeleteEmptyDictionary(manifest.RootPath);
            }
        }

        private void DeleteRemoveTaggedFiles(string path)
        {
            foreach (string removableFilePaths in Directory.GetFiles(path, $"*{Names.Extensions.Remove}", SearchOption.AllDirectories))
            {
                _removalService.TryDeleteFile(removableFilePaths);
            }
        }
    }
}