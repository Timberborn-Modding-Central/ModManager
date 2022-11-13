using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModManager.PersistenceSystem
{
    public class StrictContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract contract = base.CreateObjectContract(objectType);
            contract.ItemRequired = Required.Always;
            return contract;
        }
    }
}