using System.IO;
using Newtonsoft.Json;

namespace ModManager.PersistenceSystem
{
    public class PersistenceService : Singleton<PersistenceService>
    {
        private readonly JsonSerializerSettings _strictJsonSerializerSettings;

        private readonly JsonSerializerSettings _defaultJsonSerializerSettings;

        public PersistenceService()
        {
            _defaultJsonSerializerSettings = new JsonSerializerSettings();

            _strictJsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new StrictContractResolver()
            };
        }

        public void SaveObject(object obj, string path)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            File.WriteAllText(path, json);
        }

        public T LoadObject<T>(string path, bool strict = true)
        {
            string json = File.ReadAllText(path);

            var obj = JsonConvert.DeserializeObject<T>(json, strict ? _strictJsonSerializerSettings : _defaultJsonSerializerSettings);

            if (obj == null)
            {
                throw new JsonException($"Failed to deserialize object at: {path}");
            }

            return obj;
        }
    }
}