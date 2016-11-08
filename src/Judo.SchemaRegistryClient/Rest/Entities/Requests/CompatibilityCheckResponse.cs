namespace Judo.SchemaRegistryClient.Rest.Entities.Requests
{
    using Newtonsoft.Json;

    public class CompatibilityCheckResponse
    {

        public bool IsCompatible { get; set; }

        public static CompatibilityCheckResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<CompatibilityCheckResponse>(json);
        }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}