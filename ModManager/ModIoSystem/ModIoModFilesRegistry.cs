using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Modio.Filters;
using Modio.Models;
using Timberborn.Common;

namespace ModManager.ModIoSystem
{
    public abstract class ModIoModFilesRegistry
    {
        private static readonly Dictionary<uint, IEnumerable<File>> ModIoModFiles = new();

        public static IEnumerable<File> Get(uint modId)
        {
            lock (ModIoModFiles)
            {
                return ModIoModFiles.GetOrAdd(modId, () => Task.Run(() => RetrieveFiles(modId)).Result);
            }
        }
        
        public static Task<IEnumerable<File>> GetAsync(uint modId)
        {
            lock (ModIoModFiles)
            {
                if (ModIoModFiles.TryGetValue(modId, out var files))
                    return Task.FromResult(files);
                ModIoModFiles.Add(modId, Task.Run(() => RetrieveFiles(modId)).Result);
                return Task.FromResult(ModIoModFiles[modId]);
            }
        }
        
        public static Task<IEnumerable<File>> GetDescAsync(uint modId)
        {
            lock (ModIoModFiles)
            {
                if (ModIoModFiles.TryGetValue(modId, out var files))
                    return Task.FromResult<IEnumerable<File>>(files.OrderByDescending(file => file.Id).ToList().AsReadOnly());
                ModIoModFiles.Add(modId, Task.Run(() => RetrieveFiles(modId)).Result);
                return Task.FromResult<IEnumerable<File>>(ModIoModFiles[modId].OrderByDescending(file => file.Id).ToList().AsReadOnly());
            }
        }

        private static async Task<IEnumerable<File>> RetrieveFiles(uint modId)
        {
            return await ModIo.ModsClient[modId].Files.Search(FileFilter.Id.Desc()).ToList();
        }
    }
}