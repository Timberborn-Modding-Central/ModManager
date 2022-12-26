using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModManager
{
    public static class VersionComparer
    {
        public static bool IsVersionHigher(string version1, string version2)
        {
            if(version1 == null) { return false; }
            if(version2 == null) { return true; }
            var version1Parts = version1.Split('.');
            var version2Parts = version2.Split('.');

            for (var i = 0; i < version1Parts.Count(); i++)
            {
                if (i == version2Parts.Count() && i < version1Parts.Count())
                {
                    return true;
                }
                if (int.Parse(version1Parts[i]) > int.Parse(version2Parts[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
