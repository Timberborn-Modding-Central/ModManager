using System;
using Modio;

namespace ModManager.ModIoSystem
{
    public class ModIo
    {
        private static Client _client = null!;

        public static Client Client
        {
            get
            {
                if (_client == null)
                {
                    throw new NullReferenceException("Tried to access ModIo.Client before startup has run.");
                }

                return _client;
            }
            private set => _client = value;
        }

        public static GameClient GameClient => Client.Games[ModIoGameInfo.GameId];

        public static ModsClient ModsClient => GameClient.Mods;
        
        public static GameTagsClient GameTagsClient => GameClient.Tags;

        public static void InitializeClient(string apiKey)
        {
            Client = new Client(new Credentials(apiKey));
        }
    }
}