using Modio;
using Modio.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Timberborn.ModsSystemUI
{
    public interface IModService
    {

        ModsClient GetMods();

        GameTagsClient GetTags();

        Texture2D GetImage(Uri uri, int width, int height);

        Task<(string location, Mod Mod)> DownloadLatestMod(uint modId);

        Task<List<(string location, Mod Mod)>> DownloadDependencies(Mod mod);

        Task<List<Dependency>> GetDependencies(uint modid);
    }
}