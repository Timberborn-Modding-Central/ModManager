using System.Linq;
using UnityEngine;

namespace ModManager.VersionSystem
{
    public abstract class GameVersionGetter
    {
        private static string? _version;
        
        public static string Get()
        {
            return _version ??= Application.version.Split("-").First();
        }
    }
}