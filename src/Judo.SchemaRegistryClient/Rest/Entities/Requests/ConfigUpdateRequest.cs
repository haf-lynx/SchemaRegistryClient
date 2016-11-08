namespace Judo.SchemaRegistryClient.Rest.Entities.Requests
{
    using Newtonsoft.Json;

    public class ConfigUpdateRequest
    {
        public string Compatibility { get; set; }

        public static ConfigUpdateRequest FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ConfigUpdateRequest>(json);
        }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}