using System.Text.RegularExpressions;
using Modio.Models;

namespace ModManager.VersionSystem
{
    public static class ModIoFileExtensions
    {
        public static string? MinimumGameVersion(this File file)
        {
            if (file.Changelog == null)
                return null;
            var MinimumGameVersionLine = Regex.Match(file.Changelog, "MinimumGameVersion:(.*)");
            if (!MinimumGameVersionLine.Success)
                return null;
            
            return MinimumGameVersionLine.Groups[1].Value.Trim();
        }
        
        public static string? MaximumGameVersion(this File file)
        {
            if (file.Changelog == null)
                return null;
            var MaximumGameVersionLine = Regex.Match(file.Changelog, "MaximumGameVersion:(.*)");
            if (!MaximumGameVersionLine.Success)
                return null;
            
            return MaximumGameVersionLine.Groups[1].Value.Trim();
        }
    }
}