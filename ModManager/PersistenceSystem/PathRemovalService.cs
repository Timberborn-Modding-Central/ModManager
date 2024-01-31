using System;
using System.IO;
using System.Linq;

namespace ModManager.PersistenceSystem
{
    public class PathRemovalService : Singleton<PathRemovalService>
    {
        public void TryDeleteFile(string path)
        {
            if (! File.Exists(path))
            {
                return;
            }

            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
                // ignored: TryDelete should not crash because it's in use
            }
        }

        public void TryDeleteEmptyDictionary(string path, bool recursive = true)
        {
            if (! Directory.Exists(path))
            {
                return;
            }

            if (recursive)
            {
                foreach (var directoryPath in Directory.EnumerateDirectories(path))
                {
                    TryDeleteEmptyDictionary(directoryPath);
                }

                TryDeleteEmptyDictionary(path, false);
            }
            else
            {
                if (DirectoryContainsFiles(path))
                {
                    return;
                }

                try
                {
                    Directory.Delete(path);
                }
                catch (Exception)
                {
                    // ignored: TryDelete should not crash because it's in use
                }
            }
        }

        private bool DirectoryContainsFiles(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath).Any();
        }
    }
}