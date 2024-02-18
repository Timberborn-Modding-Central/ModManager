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
        
        public static IEnumerable<string?> CompatibleGameVersions(this File file)
        {
            var authorVersions = GetCompatibleGameVersions(file.Changelog);  
            // TODO: Decide on how to do prioritise admins/authors
            var adminVersions = GetCompatibleGameVersions(file.MetadataBlob);  
            return adminVersions;
        }
        
        public static string? MinimumGameVersion(this File file)
        {
            if (string.IsNullOrEmpty(file.Changelog))
                return null;
            var MinimumGameVersionLine = Regex.Match(file.Changelog, @"MinimumGameVersion:[^\S\r\n]*([^\s]+)");
            if (!MinimumGameVersionLine.Success)
                return null;
            
            return MinimumGameVersionLine.Groups[1].Value.Trim();
        }
        
        public static string? MaximumGameVersion(this File file)
        {
            if (string.IsNullOrEmpty(file.Changelog))
                return null;
            var MaximumGameVersionLine = Regex.Match(file.Changelog, @"MaximumGameVersion:[^\S\r\n]*([^\s]+)");
            if (!MaximumGameVersionLine.Success)
                return null;
            
            return MaximumGameVersionLine.Groups[1].Value.Trim();
        }

        private static IEnumerable<string?> GetCompatibleGameVersions(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string?>();
            var compatibleVersionArrayResult = CompatibleVersionArrayRegex.Match(text).Groups[1].Value;
            var versionMatches = CompatibleVersionRegex.Matches(compatibleVersionArrayResult);
            return versionMatches.Select(match => match.Value.Replace("\"", ""));
        }
    }
}