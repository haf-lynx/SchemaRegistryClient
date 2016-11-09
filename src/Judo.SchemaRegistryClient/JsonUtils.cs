using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Judo.SchemaRegistryClient
{
    internal static class JsonUtils
    {

        private static JsonSerializerSettings Settings = new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                };
        internal static string Serialize<T>(T graph)
        {
            return JsonConvert.SerializeObject(graph, Settings);
        }

        internal static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}