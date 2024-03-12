using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Modio.Models;

namespace ModManager.VersionSystem
{
    public static class ModIoFileExtensions
    {
        private static readonly string CompatibleVersionArrayPattern = @"CompatibleGameVersions:\s*\[\s*([\s\S]*?)\s*\]\s*";
        private static readonly Regex CompatibleVersionArrayRegex = new(CompatibleVersionArrayPattern, RegexOptions.Singleline);
        
        private static readonly string CompatibleVersionPattern = "\"(\\d+(\\.\\d+)*)+\"";
        private static readonly Regex CompatibleVersionRegex = new(CompatibleVersionPattern);
        
        private static readonly string MinimumGameVersionPattern = @"MinimumGameVersion:[^\S\r\n]*([^\s]+)";
        private static readonly Regex MinimumGameVersionRegex = new(MinimumGameVersionPattern);
        
        private static readonly string MaximumGameVersionPattern = @"MaximumGameVersion:[^\S\r\n]*([^\s]+)";
        private static readonly Regex MaximumGameVersionRegex = new(MaximumGameVersionPattern);
        
        public static IEnumerable<string?> CompatibleGameVersions(this File file)
        {
            if (string.IsNullOrEmpty(file.Changelog))
                return new List<string?>();
            if (file.Changelog.Contains("CompatibleGameVersions"))
                return GetCompatibleGameVersions(file.Changelog);
            var adminVersions = GetCompatibleGameVersions(file.MetadataBlob);  
            return adminVersions;
        }
        
        public static string? MinimumGameVersion(this File file)
        {
            if (SearchText(file.Changelog, MinimumGameVersionRegex, out var minimumGameVersion))
                return minimumGameVersion;
            if (SearchText(file.MetadataBlob, MinimumGameVersionRegex, out minimumGameVersion))
                return minimumGameVersion;
            return null;
        }
        
        public static string? MaximumGameVersion(this File file)
        {
            if (SearchText(file.Changelog, MaximumGameVersionRegex, out var maximumGameVersion))
                return maximumGameVersion;
            if (SearchText(file.MetadataBlob, MaximumGameVersionRegex, out maximumGameVersion))
                return maximumGameVersion;
            return null;
        }

        private static IEnumerable<string?> GetCompatibleGameVersions(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string?>();
            var compatibleVersionArrayResult = CompatibleVersionArrayRegex.Match(text).Groups[1].Value;
            var versionMatches = CompatibleVersionRegex.Matches(compatibleVersionArrayResult);
            return versionMatches.Select(match => match.Value.Replace("\"", ""));
        }

        private static bool SearchText(string? text, Regex regex, out string? minimumGameVersion)
        {
            minimumGameVersion = null;
            if (string.IsNullOrEmpty(text))
                return false;
            
            var changelogMinimumGameVersionLine = regex.Match(text);
            if (!changelogMinimumGameVersionLine.Success) 
                return false;
            minimumGameVersion = changelogMinimumGameVersionLine.Groups[1].Value.Trim();
            return true;
        }
    }
}