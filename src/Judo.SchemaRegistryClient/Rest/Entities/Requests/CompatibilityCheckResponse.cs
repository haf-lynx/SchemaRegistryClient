namespace Judo.SchemaRegistryClient.Rest.Entities.Requests
{
    using Newtonsoft.Json;

    public class CompatibilityCheckResponse
    {

        public bool IsCompatible { get; set; }

        public static CompatibilityCheckResponse FromJson(string json)
        {
            return JsonUtils.Deserialize<CompatibilityCheckResponse>(json);
        }


        public string ToJson()
        {
            return JsonUtils.Serialize(this);
        }

    }
}