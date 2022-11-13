
using Modio;
using Modio.Models;
using ModManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ModsSystemUI
{
    public class ModService : IModService,
                              ILoadableSingleton
    {

        private Client _client;
        private uint _timberbornGameId = 3659;

        public void Load()
        {
            //var client = new Client(new("7f52d134de5cde63fdcf163478e688e3"));
            _client = new Client(new("7f52d134de5cde63fdcf163478e688e3"));
            //_client = client.Games[3659];
        }

        public ModsClient GetMods()
        {
            return _client.Games[_timberbornGameId].Mods;
        }

        public GameTagsClient GetTags()
        {
            return _client.Games[_timberbornGameId].Tags;
        }

        public Texture2D GetImage(Uri uri, int width, int height)
        {
            using var client = new HttpClient();
            using var byteArray = client.GetByteArrayAsync(uri);
            var texture = new Texture2D(width, height);
            texture.LoadImage(byteArray.Result);
            return texture;
        }


        //public async Task<(string location, Mod Mod)> DownloadMod(uint modId)
        public async Task<(string location, Mod Mod)> DownloadLatestMod(uint modId)
        {
            var mod = await _client.Games[_timberbornGameId].Mods[modId].Get();

            Directory.CreateDirectory($"{Paths.ModManager}\\temp");
            string tempZipLocation = $"{Paths.ModManager}\\temp\\{modId}_{mod.Modfile.Id}.zip";

            await _client.Download(_timberbornGameId,
                                     modId,
                                     new FileInfo(tempZipLocation));
            (string, Mod) result = new(tempZipLocation, mod);
            return result;
        }

        private async Task<List<Dependency>> GetDependencies(uint modid)
        {
            var deps = await _client.Games[_timberbornGameId].Mods[modid].Dependencies.Get();

            List<Dependency> result = new();
            result.AddRange(deps);

            foreach (var dep in deps)
            {
                result.AddRange(await GetDependencies(dep.ModId));
            }
            return result;
        }

        public async Task<List<(string location, Mod Mod)>> DownloadDependencies(Mod mod)
        {
            var depIds = await GetDependencies(mod.Id);

            List<(string location, Mod mod)> dependencies = new();
            foreach (var dep in depIds)
            {
                dependencies.Add(await DownloadLatestMod(dep.ModId));
            }
            return dependencies;
        }

        public async Task<(string location, Mod Mod)> DownloadMod(uint modId, uint fileId)
        {
            var mod = await _client.Games[_timberbornGameId].Mods[modId].Get();
            var file = await _client.Games[_timberbornGameId].Mods[modId].Files[fileId].Get();
            mod.Modfile = file;

            Directory.CreateDirectory($"{Paths.ModManager}\\temp");
            string tempZipLocation = $"{Paths.ModManager}\\temp\\{modId}_{fileId}.zip";

            await _client.Download(_timberbornGameId,
                                   modId,
                                   fileId,
                                   new FileInfo(tempZipLocation));
            (string, Mod) result = new(tempZipLocation, mod);
            return result;
        }
    }
}