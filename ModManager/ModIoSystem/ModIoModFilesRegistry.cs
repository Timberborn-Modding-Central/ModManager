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
        private static readonly Dictionary<uint, IReadOnlyList<File>> ModIoModFiles = new();

        public static IReadOnlyList<File> Get(uint modId)
        {
            IReadOnlyList<File> list;
            lock (ModIoModFiles)
            {
                list = ModIoModFiles.GetOrAdd(modId, () => Task.Run(() => RetrieveFiles(modId)).Result);
            }
            return list;
        }
        
        public static Task<IReadOnlyList<File>> GetAsync(uint modId)
        {
            lock (ModIoModFiles)
            {
                if (ModIoModFiles.TryGetValue(modId, out var files))
                    return Task.FromResult(files);
                ModIoModFiles.Add(modId, Task.Run(() => RetrieveFiles(modId)).Result);
                return Task.FromResult(ModIoModFiles[modId]);
            }
        }
        
        public static Task<IReadOnlyList<File>> GetDescAsync(uint modId)
        {
            lock (ModIoModFiles)
            {
                if (ModIoModFiles.TryGetValue(modId, out var files))
                    return Task.FromResult<IReadOnlyList<File>>(files.OrderByDescending(file => file.Id).ToList().AsReadOnly());
                ModIoModFiles.Add(modId, Task.Run(() => RetrieveFiles(modId)).Result);
                return Task.FromResult<IReadOnlyList<File>>(ModIoModFiles[modId].OrderByDescending(file => file.Id).ToList().AsReadOnly());
            }
        }

        private static async Task<IReadOnlyList<File>> RetrieveFiles(uint modId)
        {
            return await ModIo.ModsClient[modId].Files.Search(FileFilter.Id.Desc()).ToList();
        }
    }
}