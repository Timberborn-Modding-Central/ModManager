
using Modio;
using Modio.Models;
using ModManager;
using System;
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
    }
}