
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
            Console.WriteLine($"Get mod info \"{modId}\"");
            //var file = await _client.Mods[modId].Files.Search().First();
            var mod = await _client.Games[_timberbornGameId].Mods[modId].Get();
            Console.WriteLine($"mod name \"{mod.Name}\"");

            Directory.CreateDirectory($"{Paths.ModManager}\\temp");

            string tempZipLocation = $"{Paths.ModManager}\\temp\\{modId}_{mod.Modfile.Id}.zip";
            //Console.WriteLine($"Extract to \"{tempZipLocation}\"");

            await _client.Download(_timberbornGameId,
                                     modId,
                                     new FileInfo(tempZipLocation));
            (string, Mod) result = new(tempZipLocation, mod);

            return result;
        }

        public async Task<List<Dependency>> GetDependencies(uint modid)
        {
            var deps = await _client.Games[_timberbornGameId].Mods[modid].Dependencies.Get();
            Console.WriteLine($"FOUND {deps.Count} DEPENDENCIES!!!");

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
            var deps = await _client.Games[_timberbornGameId].Mods[mod.Id].Dependencies.Get();
            Console.WriteLine($"Found {deps.Count} dependencies for {mod.Name}");

            if (deps.Count > 0)
            {
                return null;
            }

            List<(string location, Mod Mod)> results = new();

            foreach (var dep in deps)
            {
                results.Add(await DownloadLatestMod(dep.ModId));
            }

            return results;
        }

        public async Task<(string location, Mod Mod)> DownloadMod(uint modId, uint fileId)
        {
            Console.WriteLine($"Get mod info \"{modId}\"");
            var mod = await _client.Games[_timberbornGameId].Mods[modId].Get();
            var file = await _client.Games[_timberbornGameId].Mods[modId].Files[fileId].Get();
            mod.Modfile = file;

            Directory.CreateDirectory($"{Paths.ModManager}\\temp");
            string tempZipLocation = $"{Paths.ModManager}\\temp\\{modId}_{fileId}.zip";

            Console.WriteLine($"download");
            await _client.Download(_timberbornGameId,
                                   modId,
                                   fileId,
                                   new FileInfo(tempZipLocation));
            Console.WriteLine($"downloaded");
            //ModManagerPlugin.Log.LogWarning($"Downloaded zip in {tempZipLocation}");

            //var file = await _modIoClient.Games[_timberbornGameId].Mods[modId].Files[fileId].Get();
            //mod.Modfile = mod;
            (string, Mod) result = new(tempZipLocation, mod);
            return result;

            //return await DownloadMod(modId, file.Id);
        }
    }
}