using System.Linq;

namespace ModManager
{
    public static class VersionComparer
    {
        public static bool IsVersionHigher(string version1, string version2)
        {
            if (version1 == null) { return false; }
            if (version2 == null) { return true; }
            var version1Parts = version1.Split('.');
            var version2Parts = version2.Split('.');

            for (var i = 0; i < version1Parts.Count(); i++)
            {
                if (i == version2Parts.Count() && i < version1Parts.Count())
                {
                    return true;
                }
                if (int.TryParse(version1Parts[i], out int result1) &&
                    int.TryParse(version2Parts[i], out int result2))
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
    }
}
