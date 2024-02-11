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
        private static readonly Dictionary<uint, IReadOnlyList<File>> ModIoModFiles1 = new();
        private static readonly object ModIoModFilesLock = new();
        private static readonly object ModIoModFilesGetterLock = new();

        private static Dictionary<uint, IReadOnlyList<File>> ModIoModFiles
        {
            get
            {
                lock (ModIoModFilesLock)
                {
                    return ModIoModFiles1;
                }
            }
        }

        public static IReadOnlyList<File> Get(uint modId)
        {
            IReadOnlyList<File> list;
            lock (ModIoModFilesGetterLock)
            {
                list = ModIoModFiles.GetOrAdd(modId, () => Task.Run(() => RetrieveFiles(modId)).Result);
            }
            return list;
        }
        
        public static async Task<IReadOnlyList<File>> GetAsync(uint modId)
        {
            if (ModIoModFiles.TryGetValue(modId, out var files)) 
                return files;
            ModIoModFiles.Add(modId, await RetrieveFiles(modId));
            return ModIoModFiles[modId];
        }
        
        public static async Task<IReadOnlyList<File>> GetDescAsync(uint modId)
        {
            if (ModIoModFiles.TryGetValue(modId, out var files))
                return files.OrderByDescending(file => file.Id).ToList().AsReadOnly();
            ModIoModFiles.Add(modId, await RetrieveFiles(modId));
            return ModIoModFiles[modId].OrderByDescending(file => file.Id).ToList().AsReadOnly();
        }

        private static async Task<IReadOnlyList<File>> RetrieveFiles(uint modId)
        {
            return await ModIo.ModsClient[modId].Files.Search(FileFilter.Id.Desc()).ToList();
        }
    }
}