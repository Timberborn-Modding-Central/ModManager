using System;
using Modio;

namespace ModManager.ModIoSystem
{
    public class ModIo
    {
        private static ModIo? _instance;

        public ModIo()
        {
            Client = new Client(Client.ModioApiUrl, new Credentials(ModIoSecret.ApiKey));
        }

        public static ModIo Instance => _instance ??= new ModIo();

        public readonly Client Client;
    }
}