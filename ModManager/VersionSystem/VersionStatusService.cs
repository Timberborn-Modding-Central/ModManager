using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using ModManager.ModIoSystem;
using Timberborn.Common;

namespace ModManager.VersionSystem
{
    public static class VersionStatusService
    {
        private static readonly Dictionary<uint, VersionStatus> VersionStatusCache = new();

        public static VersionStatus GetVersionStatus(File? file)
        {
            if (file == null)
                return VersionStatus.Unknown;
            var version = file.Version;
            if (string.IsNullOrEmpty(version))
                return VersionStatus.Unknown;
            return VersionStatusCache.GetOrAdd(file.Id, () => FindVersionStatus(file));
        }
        
        public static VersionStatus GetVersionStatus(uint modId, string? version)
        {
            if (string.IsNullOrEmpty(version))
                return VersionStatus.Unknown;
            var file = ModIoModFilesRegistry.Get(modId).FirstOrDefault(file => file.Version == version);
            if (file == null)
                return VersionStatus.Unknown;
            if (VersionStatusCache.TryGetValue(file.Id, out var versionStatus))
                return versionStatus;
            return VersionStatusCache.GetOrAdd(file.Id, () => FindVersionStatus(file));
        }
        
        private static VersionStatus FindVersionStatus(File file)
        {
            var minimumGameVersion = file.MinimumGameVersion();
            if (string.IsNullOrEmpty(minimumGameVersion))
                return VersionStatus.Unknown;
            
            var gameVersion = GameVersionGetter.Get();
            if (VersionComparer.IsSameVersion(minimumGameVersion, gameVersion))
                return VersionStatus.Compatible;

            var maximumGameVersion = file.MaximumGameVersion();
            if (string.IsNullOrEmpty(maximumGameVersion))
                return VersionStatus.Unknown;
            
            if (VersionComparer.IsSameVersion(minimumGameVersion, gameVersion))
                return VersionStatus.Compatible;
            
            if (VersionComparer.IsVersionHigher(gameVersion, minimumGameVersion) &&
                VersionComparer.IsVersionHigher(maximumGameVersion, gameVersion))
                return VersionStatus.Compatible;

            return VersionStatus.Incompatible;
        }
    }
}