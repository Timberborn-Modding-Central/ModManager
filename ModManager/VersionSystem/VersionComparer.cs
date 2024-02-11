using System.Linq;

namespace ModManager.VersionSystem
{
    public static class VersionComparer
    {
        public static bool IsVersionHigher(string? version1, string? version2)
        {
            if (string.IsNullOrEmpty(version1)) { return false; }
            if (string.IsNullOrEmpty(version2)) { return true; }

            version1 = version1.Replace(" ", "");
            version2 = version2.Replace(" ", "");
            
            var version1Parts = version1.Split('.');
            var version2Parts = version2.Split('.');

            for (var i = 0; i < version1Parts.Count(); i++)
            {
                if (i == version2Parts.Count() && i < version1Parts.Count())
                {
                    return true;
                }
                if (int.TryParse(version1Parts[i], out var result1) &&
                    int.TryParse(version2Parts[i], out var result2))
                {
                    if (result1 > result2)
                    {
                        return true;
                    }
                    else if (result1 < result2)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        
        public static bool IsSameVersion(string? version1, string? version2)
        {
            if (version1 == null || version2 == null)
                return false;
            
            version1 = version1.Replace(" ", "");
            version2 = version2.Replace(" ", "");
            
            return version1 == version2;
        }
    }
}
